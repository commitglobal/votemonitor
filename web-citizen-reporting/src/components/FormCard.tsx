import { Card, CardContent, CardHeader, CardTitle } from "./ui/card";

export const FormCard = () => {
  return (
    <Card className="pt-0">
      <CardHeader className="flex gap-2 flex-column">
        <div className="flex flex-row items-center justify-between">
          <CardTitle className="text-xl">Form details</CardTitle>
        </div>
      </CardHeader>
      <CardContent className="flex flex-col items-baseline gap-6"></CardContent>
    </Card>
  );
};
