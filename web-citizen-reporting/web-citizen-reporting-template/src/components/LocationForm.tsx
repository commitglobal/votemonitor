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
import { useMemo, useState } from "react";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";
import type { LocationNodeModel } from "@/common/types";
import { useFormContext } from "react-hook-form";
import { FormControl, FormField, FormItem, FormMessage } from "./ui/form";
import { ErrorMessage } from "@hookform/error-message";

function LocationForm() {
  const [selectedLevel1, setSelectedLevel1] = useState<string>("");
  const [selectedLevel2, setSelectedLevel2] = useState<string>("");
  const [selectedLevel3, setSelectedLevel3] = useState<string>("");
  const [selectedLevel4, setSelectedLevel4] = useState<string>("");
  const [selectedLevel5, setSelectedLevel5] = useState<string>("");

  const { data: locationsByLevels } = useSuspenseQuery(locationsOptions());

  const form = useFormContext();

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

  function handleLocationSelect(node: LocationNodeModel | undefined) {
    console.log(node);
    form.setValue("locationId", node?.locationId ?? "");
  }

  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>Pick location for which you want to report</CardTitle>
        <CardDescription>
          Select from the hierarchical location options below
        </CardDescription>
      </CardHeader>

      <CardContent>
        <div className="flex flex-col space-y-4">
          <Select
            onValueChange={(value) => {
              handleLocationSelect(
                locationsByLevels[1]?.find((n) => n.id.toString() === value)
              );
              setSelectedLevel1(value);
              setSelectedLevel2("");
              setSelectedLevel3("");
              setSelectedLevel4("");
              setSelectedLevel5("");
            }}
            value={selectedLevel1}
          >
            <SelectTrigger className="w-full">
              <SelectValue placeholder="Location - L1" />
            </SelectTrigger>
            <SelectContent>
              <SelectGroup>
                {locationsByLevels[1]?.map((node) => (
                  <SelectItem key={node.id} value={node.id.toString()}>
                    {node.name}
                  </SelectItem>
                ))}
              </SelectGroup>
            </SelectContent>
          </Select>
        </div>

        {selectedLevel1 && filteredLevel2Nodes.length > 0 && (
          <div className="w-full pl-4 border-l-2 border-muted">
            <Select
              onValueChange={(value) => {
                handleLocationSelect(
                  filteredLevel2Nodes.find((n) => n.id.toString() === value)
                );
                setSelectedLevel2(value);
                setSelectedLevel3("");
                setSelectedLevel4("");
                setSelectedLevel5("");
              }}
              value={selectedLevel2}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Location - L2" />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  {filteredLevel2Nodes?.map((node) => (
                    <SelectItem key={node.id} value={node.id.toString()}>
                      {node.name}
                    </SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>
          </div>
        )}

        {selectedLevel2 && filteredLevel3Nodes.length > 0 && (
          <div className="w-full pl-8 border-l-2 border-muted">
            <Select
              onValueChange={(value) => {
                handleLocationSelect(
                  filteredLevel3Nodes.find((n) => n.id.toString() === value)
                );
                setSelectedLevel3(value);
                setSelectedLevel4("");
                setSelectedLevel5("");
              }}
              value={selectedLevel3}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Location - L3" />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  {filteredLevel3Nodes?.map((node) => (
                    <SelectItem key={node.id} value={node.id.toString()}>
                      {node.name}
                    </SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>
          </div>
        )}

        {selectedLevel3 && filteredLevel4Nodes.length > 0 && (
          <div className="w-full pl-12 border-l-2 border-muted">
            <Select
              onValueChange={(value) => {
                handleLocationSelect(
                  filteredLevel4Nodes.find((n) => n.id.toString() === value)
                );
                setSelectedLevel4(value);
                setSelectedLevel5("");
              }}
              value={selectedLevel4}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Location - L4" />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  {filteredLevel4Nodes?.map((node) => (
                    <SelectItem key={node.id} value={node.id.toString()}>
                      {node.name}
                    </SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>
          </div>
        )}

        {selectedLevel4 && selectedLevel4 && filteredLevel5Nodes.length > 0 && (
          <div className="w-full pl-16 border-l-2 border-muted">
            <Select
              onValueChange={(value) => {
                handleLocationSelect(
                  filteredLevel5Nodes.find((n) => n.id.toString() === value)
                );
                setSelectedLevel5(value);
              }}
              value={selectedLevel5}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Location - L5" />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  {filteredLevel5Nodes?.map((node) => (
                    <SelectItem key={node.id} value={node.id.toString()}>
                      {node.name}
                    </SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>
          </div>
        )}

        <FormField
          control={form.control}
          name="locationId"
          rules={{
            required: "Select location.",
          }}
          render={({ field }) => (
            <FormItem>
              <FormControl>
                <input {...field} type="hidden" />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <ErrorMessage errors={form.formState.errors} name="locationId" />
      </CardContent>
    </Card>
  );
}

export default LocationForm;
