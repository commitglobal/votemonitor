# Email Templates Builder

A live preview right in your browser so you don't need to keep sending real emails during development.

## Getting Started

First, install the dependencies:

```sh
pnpm install
```

Then, run the development server:

```sh
pnpm run dev
```

Open [localhost:3000](http://localhost:3000) with your browser to see the result.

## Generating templates
```sh
pnpm run export
```

## Templates lists

* [invitation for an existing user](emails/invitation-existing-user.tsx) 
* [invitation for a new user](emails/invitation-new-user.tsx)
* [confirm ](emails/confirm-email.tsx)

* [reset password](emails/reset-password.tsx)

## Images
### Local vs Prod
Images are not base64 encoded in html output so in order for them to work a CDN url should be provided.

```note
When developing localy CDN url should be empty
``` 
### Supported types
All email clients can display **.png**, **.gif**, and **.jpg** images. Unfortunately, **.svg** images are not well supported, regardless of how they're referenced, so avoid using these. See [Can I Email](https://www.caniemail.com/features/image-svg/) for more information.


## Replacement tokens
If you need a replacement token please use the format **~\$YourTokenName\$~**. 
Be aware that the replacement token value might need to be URL-encoded.