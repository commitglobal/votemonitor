import { FormFull } from "@/models/form";
import { Link } from "@tanstack/react-router";
import { FC } from "react";
import { Button } from "./ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "./ui/card";

interface FornmCardProps {
  formData: FormFull | undefined;
  languageCode: string;
}

export const FormCard: FC<FornmCardProps> = ({ formData, languageCode }) => {
  return (
    <Card className="pt-0">
      <CardHeader className="flex gap-2 flex-column">
        <CardTitle>
          {formData?.name[languageCode ?? formData.defaultLanguage]}
        </CardTitle>
        {formData?.description && (
          <CardDescription>
            {formData?.description[languageCode ?? formData.defaultLanguage]}
          </CardDescription>
        )}
      </CardHeader>
      <CardContent className="flex flex-col items-baseline gap-6">
        <Link to="/forms/$formId" params={{ formId: formData?.id }}>
          <Button>Choose</Button>
        </Link>
      </CardContent>
    </Card>
  );
};
