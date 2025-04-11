export interface GreeterProps {
  name: string;
}
function Greeter({ name }: GreeterProps) {
  return <div>Greetings {name}</div>;
}

export default Greeter;
