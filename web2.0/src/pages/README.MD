# Folder sturcture

page

## Pages naming and folder convention

- Pages should be Pascal Case eg: `HelloPage.tsx`
- Each component should have following structure

```js
export interface GreeterProps {
  name: string;
  ...
  /// rest of your props
}
// Alsways destructure your props!
function Greeter({ name }: GreeterProps) {
  return <div>Greetings {name}</div>;
}

export default Greeter;

```

- IF a page is complex add a folder called eg: `HelloPage` and use src folder sturcture for it
- src/pages/HelloPage

  - Page.tsx
  - HelloPage/assets
  - HelloPage/components
  - HelloPage/hooks

- if a component is use in more than one place move it to src/components/<area>

- src/components/<area>

  - src/components/<area>/FirstComponent.tsx
  - src/components/<area>/SecondComponent.tsx
  - src/components/<area>/ThirdComponent.tsx
