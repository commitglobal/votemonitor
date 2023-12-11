import { isEqual } from "lodash";
import { useEffect, useState } from "react";
import { validateQuestion } from "./Validation";
import { FormModel, TFormQuestionType } from "@/redux/api/types";
import { useDispatch } from "react-redux";
import { useDeleteFormMutation, useUpdateFormMutation } from "@/redux/api/formsApi";
import { useNavigate } from "react-router-dom";
import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { toast } from "react-toastify";

interface FormMenuBarProps {
  localForm: FormModel;
  form: FormModel;
  setLocalForm: (form: FormModel) => void;
  setInvalidQuestions: (invalidQuestions: string[]) => void;
  responseCount: number;
}

export default function FormMenuBar({
  localForm,
  form,
  setLocalForm,
  setInvalidQuestions,
  responseCount,
}: FormMenuBarProps) {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const [deleteForm] = useDeleteFormMutation();
  const [updateForm] = useUpdateFormMutation();
  const [audiencePrompt, setAudiencePrompt] = useState(true);
  const [isDeleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [isConfirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [isFormPublishing, setIsFormPublishing] = useState(false);
  const [isFormSaving, setIsFormSaving] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

  let faultyQuestions: string[] = [];

  useEffect(() => {
    const warningText = "You have unsaved changes - are you sure you wish to leave this page?";
    const handleWindowClose = (e: BeforeUnloadEvent) => {
      if (!isEqual(localForm, form)) {
        e.preventDefault();
        return (e.returnValue = warningText);
      }
    };

    window.addEventListener("beforeunload", handleWindowClose);
    return () => {
      window.removeEventListener("beforeunload", handleWindowClose);
    };
  }, [localForm, form]);


  const deleteFormAction = async (formId: string) => {
    try {
      deleteForm(formId);
      navigate('/forms')
    } catch (error) {
      console.log("An error occurred deleting the form");
    }
  };


  const validateForm = (form: FormModel) => {
    const existingQuestionIds = new Set();

    if (form.questions.length === 0) {
      toast.error("Please add at least one question");
      return;
    }

    faultyQuestions = [];
    for (let index = 0; index < form.questions.length; index++) {
      const question = form.questions[index];
      const isValid = validateQuestion(question);

      if (!isValid) {
        faultyQuestions.push(question.id);
      }
    }
    // if there are any faulty questions, the user won't be allowed to save the form
    if (faultyQuestions.length > 0) {
      setInvalidQuestions(faultyQuestions);
      toast.error("Please fill all required fields.");
      return false;
    }

    for (const question of form.questions) {
      const existingLogicConditions = new Set();

      if (existingQuestionIds.has(question.id)) {
        toast.error("There are 2 identical question IDs. Please update one.");
        return false;
      }
      existingQuestionIds.add(question.id);

      if (
        question.type === TFormQuestionType.MultipleChoiceSingle ||
        question.type === TFormQuestionType.MultipleChoiceMulti
      ) {
        const haveSameChoices =
          question.choices.some((element) => element.label.trim() === "") ||
          question.choices.some((element, index) =>
            question.choices
              .slice(index + 1)
              .some((nextElement) => nextElement.label.trim() === element.label.trim())
          );

        if (haveSameChoices) {
          toast.error("You have two identical choices.");
          return false;
        }
      }

      for (const logic of question.logic || []) {
        const validFields = ["condition", "destination", "value"].filter(
          (field) => logic[field] !== undefined
        ).length;

        if (validFields < 2) {
          setInvalidQuestions([question.id]);
          toast.error("Incomplete logic jumps detected: Please fill or delete them.");
          return false;
        }

        const thisLogic = `${logic.condition}-${logic.value}`;
        if (existingLogicConditions.has(thisLogic)) {
          setInvalidQuestions([question.id]);
          toast.error("You have 2 competing logic conditions. Please update or delete one.");
          return false;
        }
        existingLogicConditions.add(thisLogic);
      }
    }

    return true;
  };

  const saveFormAction = async (shouldNavigateBack = false) => {
    if (localForm.questions.length === 0) {
      toast.error("Please add at least one question.");
      return;
    }
    setIsFormSaving(true);
    // Create a copy of localForm with isDraft removed from every question
    const strippedForm: FormModel = {
      ...localForm,
      questions: localForm.questions.map((question) => {
        const { isDraft, ...rest } = question;
        return rest;
      }),
    };

    if (!validateForm(localForm)) {
      setIsFormSaving(false);
      return;
    }

    try {
      await updateForm({ ...strippedForm }).unwrap();

      setIsFormSaving(false);
      toast.success("Changes saved.");
      if (shouldNavigateBack) {
        navigate('/forms');
      }
    } catch (e) {
      console.error(e);
      setIsFormSaving(false);
      toast.error(`Error saving changes`);
      return;
    }
  };

  return (
    <>

      <div className="border-b border-slate-200 bg-white px-5 py-3 sm:flex sm:items-center sm:justify-between">
        <div className="flex items-center space-x-2 whitespace-nowrap">
          <Button
            onClick={() => {
              alert('back')
              // handleBack();
            }}>
            Back
          </Button>
          <Input
            defaultValue={localForm.code}
            onChange={(e) => {
              const updatedForm = { ...localForm, name: e.target.value };
              setLocalForm(updatedForm);
            }}
            className="w-72 border-white hover:border-slate-200 "
          />
        </div>
        {responseCount > 0 && (
          <div className="mx-auto flex items-center rounded-full border border-amber-200 bg-amber-100 p-2 text-amber-700 shadow-sm">
            <p className=" pl-1 text-xs lg:text-sm">
              This form received responses, make changes with caution.
            </p>
          </div>
        )}
        <div className="mt-3 flex sm:ml-4 sm:mt-0">

          <Button
            disabled={isFormPublishing}
            className="mr-3"
            onClick={() => saveFormAction(false)}>
            Save
          </Button>

        </div>
        {/* <DeleteDialog
          deleteWhat="Draft"
          open={isDeleteDialogOpen}
          setOpen={setDeleteDialogOpen}
          onDelete={async () => {
            setIsDeleting(true);
            await deleteForm(localForm.id);
            setIsDeleting(false);
          }}
          text="Do you want to delete this draft?"
          isDeleting={isDeleting}
          isSaving={isSaving}
          useSaveInsteadOfCancel={true}
          onSave={async () => {
            setIsSaving(true);
            await saveFormAction(true);
            setIsSaving(false);
          }}
        />
        <AlertDialog
          confirmWhat="Form changes"
          open={isConfirmDialogOpen}
          setOpen={setConfirmDialogOpen}
          onDiscard={() => {
            setConfirmDialogOpen(false);
            router.back();
          }}
          text="You have unsaved changes in your form. Would you like to save them before leaving?"
          confirmButtonLabel="Save"
          onSave={() => saveFormAction(true)}
        /> */}
      </div>
    </>
  );
}
