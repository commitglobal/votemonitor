import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { cn } from "@/lib/utils";
import { Link } from "@tanstack/react-router";
import type { FC } from "react";
import { Button } from "./ui/button";

interface NotificationProps {
  id: string;
  sentAt: Date;
  title: string;
  body: string;
  isInsideList?: boolean;
}

export const Notification: FC<NotificationProps> = ({
  id,
  sentAt,
  title,
  body,
  isInsideList,
}) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
        {/* convert to local time from utc */}
        <CardDescription>
          Sent at: {new Date(sentAt).toLocaleString("en-GB")}
        </CardDescription>
      </CardHeader>
      <CardContent>
        <div
          className={cn({ "line-clamp-2": isInsideList })}
          dangerouslySetInnerHTML={{ __html: body }}
        />
      </CardContent>
      <CardFooter>
        {isInsideList && (
          <Link
            title="Read more"
            to="/notifications/$notificationId"
            params={{ notificationId: id }}
          >
            <Button>Read more</Button>
          </Link>
        )}
      </CardFooter>
    </Card>
  );
};

export default Notification;
