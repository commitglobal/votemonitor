```
 /$$$$$$$$                         /$$ /$$       /$$$$$$$$                                /$$             /$$
| $$_____/                        |__/| $$      |__  $$__/                               | $$            | $$
| $$       /$$$$$$/$$$$   /$$$$$$  /$$| $$         | $$  /$$$$$$  /$$$$$$/$$$$   /$$$$$$ | $$  /$$$$$$  /$$$$$$    /$$$$$$   /$$$$$$$
| $$$$$   | $$_  $$_  $$ |____  $$| $$| $$         | $$ /$$__  $$| $$_  $$_  $$ /$$__  $$| $$ |____  $$|_  $$_/   /$$__  $$ /$$_____/
| $$__/   | $$ \ $$ \ $$  /$$$$$$$| $$| $$         | $$| $$$$$$$$| $$ \ $$ \ $$| $$  \ $$| $$  /$$$$$$$  | $$    | $$$$$$$$|  $$$$$$
| $$      | $$ | $$ | $$ /$$__  $$| $$| $$         | $$| $$_____/| $$ | $$ | $$| $$  | $$| $$ /$$__  $$  | $$ /$$| $$_____/ \____  $$
| $$$$$$$$| $$ | $$ | $$|  $$$$$$$| $$| $$         | $$|  $$$$$$$| $$ | $$ | $$| $$$$$$$/| $$|  $$$$$$$  |  $$$$/|  $$$$$$$ /$$$$$$$/
|________/|__/ |__/ |__/ \_______/|__/|__/         |__/ \_______/|__/ |__/ |__/| $$____/ |__/ \_______/   \___/   \_______/|_______/
                                                                               | $$
                                                                               | $$
                                                                               |__/
```


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

## From Submissions
after generating html files remove `<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">` from all fragments!

### Input answer
This fragment is used for TextQuestion, NumberQuestion, DateQuestion

Replace **~\$text\$~** with question text, replace **~\$answer\$~**. with answer in [input-answer-fragment.html](./out/components/form-submission/input-answer-fragment.html)

### Rating answer
Replace **~\$text\$~** with question text

Replace **~\$value\$~** in [rating-answer-entry-selected-fragment.html](./out/components/form-submission/rating-answer-entry-selected-fragment.html) or [rating-answer-entry-fragment.html](./out/components/form-submission/rating-answer-entry-fragment.html) depending on the answer.

Concatenate entry fragments and replace **~\$options\$~** in [rating-answer-fragment.html](./out/components/form-submission/rating-answer-fragment.html)

### Select answer
This fragment is used for SingleSelectQuestion or MultiSelectQuestion
Replace **~\$text\$~** with question text

Replace **~\$value\$~** in [select-answer-entry-selected-fragment.html](./out/components/form-submission/select-answer-entry-selected-fragment.html) or [select-answer-entry-fragment.html](./out/components/form-submission/select-answer-entry-fragment.html) depending on the answer. Concatenate entry fragments and replace **~\$options\$~** in [select-answer-fragment.html](./out/components/form-submission/select-answer-fragment.html)

