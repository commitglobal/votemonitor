import { questionSchema, textQuestionSchema } from "@/components/form-builder/shared";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";
import { withFieldGroup } from "@/hooks/form";
import { DisplayLogicCondition, QuestionType } from "@/types/form";
import { useStore } from "@tanstack/react-form";
import { ChevronDownIcon } from "lucide-react";
import { useState } from "react";
import z from "zod";


const defaultValues: z.infer<typeof textQuestionSchema> = {
  questionId: crypto.randomUUID(),
  $questionType: QuestionType.TextQuestionType,
  text: "Text Question Number 1",
  helptext: "",
  inputPlaceholder: "",
  code: "",
  hasDisplayLogic: false,
  condition: DisplayLogicCondition.Equals,
  value: "",
  parentQuestionId: "",
};


const displayLogicConditions: {
  [questionType: string]: DisplayLogicCondition[];
} = {
  [QuestionType.MultiSelectQuestionType]: [DisplayLogicCondition.Includes],
  [QuestionType.SingleSelectQuestionType]: [DisplayLogicCondition.Includes],
  [QuestionType.NumberQuestionType]: [DisplayLogicCondition.Equals, DisplayLogicCondition.NotEquals, DisplayLogicCondition.LessThan, DisplayLogicCondition.LessEqual, DisplayLogicCondition.GreaterThan, DisplayLogicCondition.GreaterEqual],
  [QuestionType.RatingQuestionType]: [DisplayLogicCondition.Equals, DisplayLogicCondition.NotEquals, DisplayLogicCondition.LessThan, DisplayLogicCondition.LessEqual, DisplayLogicCondition.GreaterThan, DisplayLogicCondition.GreaterEqual],
};



export const TextQuestionCard = withFieldGroup({
  defaultValues,
  props: {
    onMoveUp: () => { },
    onMoveDown: () => { },
    onRemove: () => { },
    index: 0,
    numberOfQuestions: 0,
    questions: [] as z.infer<typeof questionSchema>[]
  },
  render: function Render({ group, index, numberOfQuestions, onMoveUp, onMoveDown, onRemove, questions }) {
    const [open, setOpen] = useState(false);
    const questionText = useStore(group.store, (state) => state.values.text)
    const code = useStore(group.store, (state) => state.values.code)

    return (
      <Card>
        <Collapsible open={open} onOpenChange={setOpen}>
          <CollapsibleTrigger asChild>
            <button
              type="button"
              className="flex w-full items-center justify-between px-6 py-4 text-left hover:bg-accent/50 transition-colors"
            >
              <span className="font-medium">
                {code || `#${index + 1}`} - {questionText || "Untitled"}
              </span>
              <ChevronDownIcon
                className={`h-4 w-4 transition-transform ${open ? "rotate-180" : ""}`}
              />
            </button>
          </CollapsibleTrigger>
          <CollapsibleContent>
            <CardContent className="space-y-4">
              <div className="flex flex-col sm:flex-row gap-2">
                <div className="w-full sm:w-[64px]">
                  <group.AppField name="code">
                    {(subField) => {
                      return (
                        <subField.TextInput
                          label="Code"
                          id={`questions-${index}-code`}
                          type="text"
                        />
                      );
                    }}
                  </group.AppField>
                </div>

                <div className="w-full sm:flex-1">
                  <group.AppField name="text">
                    {(subField) => {
                      return (
                        <subField.TextInput
                          label="Text"
                          id={`questions-${index}-text`}
                          type="text"
                        />
                      );
                    }}
                  </group.AppField>
                </div>
              </div>

              <group.AppField name="helptext">
                {(subField) => {
                  return (
                    <subField.TextInput
                      label="Helptext"
                      id={`questions-${index}-helptext`}
                      type="text"
                    />
                  );
                }}
              </group.AppField>

              <group.AppField name="inputPlaceholder">
                {(subField) => {
                  return (
                    <subField.TextInput
                      label="Input Placeholder"
                      id={`questions-${index}-inputPlaceholder`}
                      type="text"
                    />
                  );
                }}
              </group.AppField>

              <group.AppField name="hasDisplayLogic">
                {(field) => {
                  return (
                    <field.Toggle
                      label="Has Display Logic"
                      id="hasDisplayLogic"
                      description="Whether the question should be displayed based on the parent question."
                      onChange={(value) => {
                        if (!value) {
                          group.setFieldValue('parentQuestionId', '')
                          group.setFieldValue('condition', DisplayLogicCondition.Equals)
                          group.setFieldValue('value', '')
                        }
                      }}
                    />
                  );
                }}
              </group.AppField>
              <group.Subscribe selector={(state) => ({hasDisplayLogic:state.values.hasDisplayLogic,parentQuestionId:state.values.parentQuestionId})}>
                {({hasDisplayLogic,parentQuestionId}) => {
                  if (!hasDisplayLogic) return null;

                  const parentQuestion = questions.find((question) => question.questionId === parentQuestionId);
                  // if (!parentQuestion) return null;
                  console.log(parentQuestionId);
                  console.log(parentQuestion);
                  console.log(parentQuestion?.$questionType);
                  console.log(displayLogicConditions[parentQuestion?.$questionType as QuestionType]);

                  return (
                    <div>
                      <group.AppField name="parentQuestionId">
                        {(field) => {
                          return (
                            <field.Select
                              label="Parent Question ID"
                              id="parentQuestionId"
                              placeholder="Select a parent question"
                              options={questions.map((question) => ({
                                value: question.questionId,
                                label: question.text,
                              }))}
                            />
                          );
                        }}
                      </group.AppField>

                      <group.AppField name="condition">
                        {(field) => {
                          return (
                            <field.Select
                              label="Condition"
                              id="condition"
                              placeholder="Select a condition"
                              disabled={!parentQuestionId}
                              options={displayLogicConditions[parentQuestion?.$questionType as QuestionType]?.map((condition) => ({
                                value: condition,
                                label: condition,
                              }))??[]}
                            />
                          );
                        }}
                      </group.AppField>

                      <group.AppField name="value">
                        {(field) => {
                          return (
                            <field.TextInput
                              label="Value"
                              id="value"
                              type="text"
                              description="The value of the display logic."
                              disabled={!parentQuestionId}
                            />
                          );
                        }}
                      </group.AppField>
                    </div>
                  );
                }}
              </group.Subscribe>

              <div className="flex flex-row gap-2">
                <Button
                  variant="destructive"
                  type="button"
                  onClick={onRemove}
                >
                  Remove
                </Button>

                <Button
                  variant="outline"
                  type="button"
                  onClick={onMoveUp}
                  disabled={index === 0}
                >
                  move up
                </Button>

                <Button
                  variant="outline"
                  type="button"
                  onClick={onMoveDown}
                  disabled={index === numberOfQuestions - 1}
                >
                  move down
                </Button>
              </div>

            </CardContent>
          </CollapsibleContent>
        </Collapsible>
      </Card>
    );
  }
});