"use client";

import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { locationsOptions } from "@/queries/use-locations";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useCallback, useMemo } from "react";
import { useFormContext } from "react-hook-form";
import { z } from "zod";
import { FormControl, FormField, FormItem, FormMessage } from "./ui/form";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";

export const locationSchema = z.object({
  locationId: z.string().min(1, "Location is required"),
  selectedLevel1: z.string().catch(""),
  selectedLevel2: z.string().catch(""),
  selectedLevel3: z.string().catch(""),
  selectedLevel4: z.string().catch(""),
  selectedLevel5: z.string().catch(""),
});

type LocationFormValues = z.infer<typeof locationSchema>;

function ReportLocationStep() {
  const { data } = useSuspenseQuery(locationsOptions());
  const { watch, reset, control } = useFormContext<LocationFormValues>();
  const formValues = watch();

  const filteredLevel2Nodes = useMemo(
    () =>
      data?.[2]?.filter(
        (node) => node.parentId?.toString() === formValues?.selectedLevel1
      ),
    [data, formValues?.selectedLevel1]
  );

  const filteredLevel3Nodes = useMemo(
    () =>
      data?.[3]?.filter(
        (node) => node.parentId?.toString() === formValues?.selectedLevel2
      ),
    [data, formValues?.selectedLevel2]
  );

  const filteredLevel4Nodes = useMemo(
    () =>
      data?.[4]?.filter(
        (node) => node.parentId?.toString() === formValues?.selectedLevel3
      ),
    [data, formValues?.selectedLevel3]
  );

  const filteredLevel5Nodes = useMemo(
    () =>
      data?.[5]?.filter(
        (node) => node.parentId?.toString() === formValues?.selectedLevel4
      ),
    [data, formValues?.selectedLevel4]
  );

  const updateForm = useCallback(
    (newValues: LocationFormValues) => {
      reset({
        ...newValues,
      });
    },
    [reset]
  );

  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>Pick location for which you want to report</CardTitle>
        <CardDescription>
          Select from the hierarchical location options below
        </CardDescription>
      </CardHeader>

      <CardContent className="w-full flex flex-col gap-4">
        <Select
          onValueChange={(value) => {
            const location = data?.[1]?.find(
              (node) => node.id?.toString() === value
            );

            updateForm({
              locationId: location?.locationId ?? "",
              selectedLevel1: value,
              selectedLevel2: "",
              selectedLevel3: "",
              selectedLevel4: "",
              selectedLevel5: "",
            });
          }}
          value={formValues.selectedLevel1 ?? ""}
        >
          <SelectTrigger className="w-full md:w-1/2">
            <SelectValue placeholder="Location - L1" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {data?.[1]?.map((node) => (
                <SelectItem key={node.id} value={node.id?.toString()}>
                  {node.name}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>

        <Select
          disabled={!formValues.selectedLevel1 || !filteredLevel2Nodes?.length}
          onValueChange={(value) => {
            const location = data?.[2]?.find(
              (node) => node.id?.toString() === value
            );

            updateForm({
              locationId: location?.locationId ?? "",
              selectedLevel1: formValues.selectedLevel1,
              selectedLevel2: value,
              selectedLevel3: "",
              selectedLevel4: "",
              selectedLevel5: "",
            });
          }}
          value={formValues.selectedLevel2 ?? ""}
        >
          <SelectTrigger className="w-full md:w-1/2">
            <SelectValue placeholder="Location - L2" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {filteredLevel2Nodes?.map((node) => (
                <SelectItem key={node.id} value={node.id?.toString()}>
                  {node.name}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>

        <Select
          disabled={!formValues.selectedLevel2 || !filteredLevel3Nodes?.length}
          onValueChange={(value) => {
            const location = data?.[3]?.find(
              (node) => node.id?.toString() === value
            );

            updateForm({
              locationId: location?.locationId ?? "",
              selectedLevel1: formValues.selectedLevel1,
              selectedLevel2: formValues.selectedLevel2,
              selectedLevel3: value,
              selectedLevel4: "",
              selectedLevel5: "",
            });
          }}
          value={formValues.selectedLevel3 ?? ""}
        >
          <SelectTrigger className="w-full md:w-1/2">
            <SelectValue placeholder="Location - L3" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {filteredLevel3Nodes?.map((node) => (
                <SelectItem key={node.id} value={node.id?.toString()}>
                  {node.name}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>

        <Select
          disabled={!formValues.selectedLevel3 || !filteredLevel4Nodes?.length}
          onValueChange={(value) => {
            const location = data?.[4]?.find(
              (node) => node.id?.toString() === value
            );

            updateForm({
              locationId: location?.locationId ?? "",
              selectedLevel1: formValues.selectedLevel1,
              selectedLevel2: formValues.selectedLevel2,
              selectedLevel3: formValues.selectedLevel3,
              selectedLevel4: value,
              selectedLevel5: "",
            });
          }}
          value={formValues.selectedLevel4 ?? ""}
        >
          <SelectTrigger className="w-full md:w-1/2">
            <SelectValue placeholder="Location - L4" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {filteredLevel4Nodes?.map((node) => (
                <SelectItem key={node.id} value={node.id?.toString()}>
                  {node.name}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>

        <Select
          disabled={!formValues.selectedLevel4 || !filteredLevel5Nodes?.length}
          onValueChange={(value) => {
            const location = data?.[5]?.find(
              (node) => node.id?.toString() === value
            );
            updateForm({
              locationId: location?.locationId ?? "",
              selectedLevel1: formValues.selectedLevel1,
              selectedLevel2: formValues.selectedLevel2,
              selectedLevel3: formValues.selectedLevel3,
              selectedLevel4: formValues.selectedLevel4,
              selectedLevel5: value,
            });
          }}
          value={formValues.selectedLevel5 ?? ""}
        >
          <SelectTrigger className="w-full md:w-1/2">
            <SelectValue placeholder="Location - L5" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {filteredLevel5Nodes?.map((node) => (
                <SelectItem key={node.id} value={node.id?.toString()}>
                  {node.name}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>

        <FormField
          control={control}
          name="locationId"
          render={({ field }) => (
            <FormItem>
              <FormControl>
                <input {...field} type="hidden" />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </CardContent>
    </Card>
  );
}

export default ReportLocationStep;
