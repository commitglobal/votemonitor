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
import { useEffect, useMemo } from "react";
import { useFormContext } from "react-hook-form";
import { z } from "zod";
import { FormControl, FormField, FormItem, FormMessage } from "./ui/form";
import {
  Select,
  SelectContent,
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
  const { data: locationsByLevels } = useSuspenseQuery(locationsOptions());

  const form = useFormContext<LocationFormValues>();

  const selectedLevel1 = form.watch("selectedLevel1");
  const selectedLevel2 = form.watch("selectedLevel2");
  const selectedLevel3 = form.watch("selectedLevel3");
  const selectedLevel4 = form.watch("selectedLevel4");
  const selectedLevel5 = form.watch("selectedLevel5");

  const selectedLevel1Node = useMemo(() => {
    return locationsByLevels[1]?.find(
      (n) => n.id.toString() === selectedLevel1
    );
  }, [locationsByLevels, selectedLevel1]);

  const selectedLevel2Node = useMemo(() => {
    return locationsByLevels[2]?.find(
      (n) => n.id.toString() === selectedLevel2
    );
  }, [locationsByLevels, selectedLevel1, selectedLevel2]);

  const selectedLevel3Node = useMemo(() => {
    return locationsByLevels[3]?.find(
      (n) => n.id.toString() === selectedLevel3
    );
  }, [locationsByLevels, selectedLevel1, selectedLevel2, selectedLevel3]);

  const selectedLevel4Node = useMemo(() => {
    return locationsByLevels[4]?.find(
      (n) => n.id.toString() === selectedLevel4
    );
  }, [
    locationsByLevels,
    selectedLevel1,
    selectedLevel2,
    selectedLevel3,
    selectedLevel4,
  ]);

  const selectedLevel5Node = useMemo(() => {
    return locationsByLevels[5]?.find(
      (n) => n.id.toString() === selectedLevel5
    );
  }, [
    locationsByLevels,
    selectedLevel1,
    selectedLevel2,
    selectedLevel3,
    selectedLevel4,
    selectedLevel5,
  ]);

  const filteredLevel2Nodes = useMemo(
    () =>
      locationsByLevels[2]?.filter(
        (node) => node.parentId?.toString() === selectedLevel1
      ),
    [locationsByLevels, selectedLevel1]
  );

  const filteredLevel3Nodes = useMemo(
    () =>
      locationsByLevels[3]?.filter(
        (node) => node.parentId?.toString() === selectedLevel2
      ),
    [locationsByLevels, selectedLevel2]
  );

  const filteredLevel4Nodes = useMemo(
    () =>
      locationsByLevels[4]?.filter(
        (node) => node.parentId?.toString() === selectedLevel3
      ),
    [locationsByLevels, selectedLevel3]
  );

  const filteredLevel5Nodes = useMemo(
    () =>
      locationsByLevels[5]?.filter(
        (node) => node.parentId?.toString() === selectedLevel4
      ),
    [locationsByLevels, selectedLevel4]
  );

  useEffect(() => {
    form.setValue("locationId", "");
    form.setValue("selectedLevel2", "");
    form.setValue("selectedLevel3", "");
    form.setValue("selectedLevel4", "");
    form.setValue("selectedLevel5", "");
  }, [selectedLevel1]);

  useEffect(() => {
    form.setValue("locationId", "");
    form.setValue("selectedLevel3", "");
    form.setValue("selectedLevel4", "");
    form.setValue("selectedLevel5", "");
  }, [selectedLevel1, selectedLevel2]);

  useEffect(() => {
    form.setValue("locationId", "");
    form.setValue("selectedLevel4", "");
    form.setValue("selectedLevel5", "");
  }, [selectedLevel1, selectedLevel2, selectedLevel3]);

  useEffect(() => {
    form.setValue("locationId", "");
    form.setValue("selectedLevel5", "");
  }, [selectedLevel1, selectedLevel2, selectedLevel3, selectedLevel4]);

  useEffect(() => {
    form.setValue(
      "locationId",
      selectedLevel5Node?.locationId ??
        selectedLevel4Node?.locationId ??
        selectedLevel3Node?.locationId ??
        selectedLevel2Node?.locationId ??
        selectedLevel1Node?.locationId ??
        ""
    );
  }, [
    selectedLevel1,
    selectedLevel2,
    selectedLevel3,
    selectedLevel4,
    selectedLevel5,
  ]);

  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>Pick location for which you want to report</CardTitle>
        <CardDescription>
          Select from the hierarchical location options below
        </CardDescription>
      </CardHeader>

      <CardContent className="w-full flex flex-col gap-4">
        <FormField
          control={form.control}
          name="selectedLevel1"
          render={({ field }) => (
            <FormItem>
              <Select onValueChange={field.onChange} defaultValue={field.value}>
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder="Location level 1" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {locationsByLevels[1]?.map((node) => (
                    <SelectItem key={node.id} value={node.id.toString()}>
                      {node.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />

        {selectedLevel1 && filteredLevel2Nodes.length > 0 && (
          <div className="w-full pl-4 border-l-2 border-muted">
            <FormField
              control={form.control}
              name="selectedLevel2"
              render={({ field }) => (
                <FormItem>
                  <Select
                    onValueChange={field.onChange}
                    defaultValue={field.value}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Location level 2" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {filteredLevel2Nodes.map((node) => (
                        <SelectItem key={node.id} value={node.id.toString()}>
                          {node.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
        )}

        {selectedLevel2 && filteredLevel3Nodes.length > 0 && (
          <div className="w-full pl-8 border-l-2 border-muted">
            <FormField
              control={form.control}
              name="selectedLevel3"
              render={({ field }) => (
                <FormItem>
                  <Select
                    onValueChange={field.onChange}
                    defaultValue={field.value}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Location level 3" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {filteredLevel3Nodes.map((node) => (
                        <SelectItem key={node.id} value={node.id.toString()}>
                          {node.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
        )}

        {selectedLevel3 && filteredLevel4Nodes.length > 0 && (
          <div className="w-full pl-12 border-l-2 border-muted">
            <FormField
              control={form.control}
              name="selectedLevel4"
              render={({ field }) => (
                <FormItem>
                  <Select
                    onValueChange={field.onChange}
                    defaultValue={field.value}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Location level 4" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {filteredLevel4Nodes.map((node) => (
                        <SelectItem key={node.id} value={node.id.toString()}>
                          {node.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
        )}

        {selectedLevel4 && filteredLevel5Nodes.length > 0 && (
          <div className="w-full pl-16 border-l-2 border-muted">
            <FormField
              control={form.control}
              name="selectedLevel5"
              render={({ field }) => (
                <FormItem>
                  <Select
                    onValueChange={field.onChange}
                    defaultValue={field.value}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Location level 5" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {filteredLevel5Nodes.map((node) => (
                        <SelectItem key={node.id} value={node.id.toString()}>
                          {node.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
        )}

        <FormField
          control={form.control}
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
