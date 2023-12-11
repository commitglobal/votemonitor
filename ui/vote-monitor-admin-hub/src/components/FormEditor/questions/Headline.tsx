interface HeadlineProps {
  headline?: string;
  questionId: string;
  alignTextCenter?: boolean;
}

export default function Headline({
  headline,
  questionId,
  alignTextCenter = false,
}: HeadlineProps) {
  return (
    <label htmlFor={questionId} className="text-heading mb-1.5 block text-base font-semibold leading-6">
      <div
        className={`flex items-center  ${alignTextCenter ? "justify-center" : "mr-[3ch] justify-between"}`}>
        {headline}
      </div>
    </label>
  );
}
