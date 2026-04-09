import { isDateQuestion, isMultiSelectQuestion, isNumberQuestion, isRatingQuestion, isSingleSelectQuestion, isTextQuestion } from "@/common/guards";
import { BaseQuestion } from "@/common/types";
import { FormTemplateFull } from "@/features/form-templates/models";
import { FormFull } from "@/features/forms/models";
import {
  BorderStyle,
  Document,
  FileChild,
  HeadingLevel,
  Paragraph,
  Table,
  TableCell,
  TableRow,
  TextRun,
  WidthType
} from "docx";
import { ratingScaleToNumber } from "./utils";

export class FormExporter {
  public create(form: FormFull | FormTemplateFull): Document {
    const document = new Document({
      sections: [
        {
          children: [
            ...form.languages.flatMap(language => this.createFormContent(form, language))
          ]
        }
      ]
    });

    return document;
  }

  private createFormContent(form: FormFull | FormTemplateFull, language: string): FileChild[] {
    return [
      new Paragraph({
        text: `${form.code} - ${form.name[language]}`,
        heading: HeadingLevel.TITLE
      }),
      new Paragraph({
        text: `${form.description?.[language]}`,
        heading: HeadingLevel.HEADING_1,
      }),

      ...this.createObservationSection(),
      this.createHeading("Questions"),
      ...form.questions.flatMap(question => this.createQuestionSection(question, form.questions, language)),
      ...this.createNotesSection(42),

    ];
  }

  private createObservationSection(): FileChild[] {
    return [
      new Paragraph({
        text: `Observation details`,
        heading: HeadingLevel.HEADING_2,
      }),

      new Table({
        width: {
          size: 100,
          type: WidthType.PERCENTAGE,
        },
        rows: [
          this.createRow("Polling Station"),
          this.createRow("Arrival time"),
          this.createRow("Departure time"),
        ],
      }),
    ];
  }

  private createRow(label: string): TableRow {
    return new TableRow({
      children: [
        // LEFT: label
        new TableCell({
          width: { size: 40, type: WidthType.PERCENTAGE },
          borders: { top: { style: BorderStyle.NIL }, bottom: { style: BorderStyle.NIL }, left: { style: BorderStyle.NIL }, right: { style: BorderStyle.NIL } },
          children: [
            new Paragraph({
              children: [new TextRun({ text: label, bold: true })],
            }),
          ],
        }),

        // RIGHT: input line
        new TableCell({
          width: { size: 60, type: WidthType.PERCENTAGE },
          borders: {
            bottom: {
              style: BorderStyle.SINGLE,
              color: "000000",
            },
          },
          children: [new Paragraph("")],
        }),
      ],
    });
  }


  // Add a new page with heading "Notes" and empty underscored lines
  private createNotesSection(lineCount: number = 20): FileChild[] {
    return [
      new Paragraph({
        children: [],
        pageBreakBefore: true, // starts a new page
      }),
      new Paragraph({
        text: `Notes`,
        heading: HeadingLevel.HEADING_2,
      }),

      new Table({
        width: {
          size: 100,
          type: WidthType.PERCENTAGE,
        },
        rows: [
          ...Array.from({ length: lineCount }, () => new TableRow({
            children: [
              // LEFT: label
              new TableCell({
                width: { size: 40, type: WidthType.PERCENTAGE },
                borders: { top: { style: BorderStyle.NIL }, bottom: { style: BorderStyle.SINGLE, color: "000000", size: 1 }, left: { style: BorderStyle.NIL }, right: { style: BorderStyle.NIL } },
                children: [
                  new Paragraph({
                    children: [new TextRun({ text: "" })],
                  }),
                ],
              }),
            ]
          })),
        ],
      }),
    ];
  }

  // Helper to get display logic text
  private getDisplayLogicText(
    question: BaseQuestion,
    allQuestions: BaseQuestion[],
    language: string
  ): string {
    if (!question.displayLogic) return '';

    const logic = question.displayLogic;

    // Find parent question
    const parent = allQuestions.find(q => q.id === logic.parentQuestionId);
    if (!parent) return '';

    let valueText = logic.value;

    // If the condition is 'Includes', get the option text from parent
    if (logic.condition === 'Includes') {
      if (isSingleSelectQuestion(parent) || isMultiSelectQuestion(parent)) {
        const option = parent.options.find(opt => opt.id === logic.value);
        if (option) {
          valueText = option.text[language] ?? '';
        }
      }
    }

    // Build the message
    switch (logic.condition) {
      case 'Equals':
        return `Fill in when ${parent.code} equals "${valueText}"`;
      case 'NotEquals':
        return `Fill in when ${parent.code} is not "${valueText}"`;
      case 'LessThan':
        return `Fill in when ${parent.code} is less than "${valueText}"`;
      case 'LessEqual':
        return `Fill in when ${parent.code} is less than or equal to "${valueText}"`;
      case 'GreaterThan':
        return `Fill in when ${parent.code} is greater than "${valueText}"`;
      case 'GreaterEqual':
        return `Fill in when ${parent.code} is greater than or equal to "${valueText}"`;
      case 'Includes':
        return `Fill in when ${parent.code} includes "${valueText}"`;
      default:
        return '';
    }
  }


  private createQuestionSection(question: BaseQuestion, allQuestions: BaseQuestion[], language: string): FileChild[] {
    const children: FileChild[] = [];

    // Question text
    children.push(new Paragraph({
      children: [
        new TextRun({ text: `${question.code}. `, bold: true }),
        new TextRun({ text: question.text[language] })
      ],
    }));

    // Helptext (if present)
    if (question.helptext?.[language]) {
      children.push(new Paragraph({
        children: [new TextRun({ text: question.helptext[language], italics: true })],
      }));
    }

    // Display logic (if present)
    if (question.displayLogic) {
      children.push(new Paragraph({
        children: [new TextRun({ text: this.getDisplayLogicText(question, allQuestions, language), italics: true })],

      }));
    }

    if (isTextQuestion(question)) {
      const lineCount = 3;
      children.push(
        new Table({
          width: {
            size: 100,
            type: WidthType.PERCENTAGE,
          },
          rows: Array.from({ length: lineCount }, () => {
            return new TableRow({
              children: [
                new TableCell({
                  borders: {
                    top: { style: BorderStyle.NIL, color: "000000", size: 1 },
                    bottom: { style: BorderStyle.SINGLE, color: "000000", size: 1 },
                    left: { style: BorderStyle.NIL, color: "000000", size: 1 },
                    right: { style: BorderStyle.NIL, color: "000000", size: 1 },
                  },
                  children: [new Paragraph({ text: "" })],
                }),
              ],
            });
          }),
        })
      );
    }

    if (isNumberQuestion(question)) {
      children.push(
        new Paragraph({
          border: {
            bottom: {
              color: "000000",
              space: 1,
              style: BorderStyle.SINGLE,
              size: 1
            },
          },
          children: [new TextRun("")],
        })
      );
    }

    if (isDateQuestion(question)) {
      children.push(
        new Paragraph({
          border: {
            bottom: {
              color: "000000",
              space: 1,
              style: BorderStyle.SINGLE,
              size: 1
            },
          },
          spacing: {
            after: 200, // space between lines
          },
          children: [new TextRun("")],
        })
      );
    }

    if (isSingleSelectQuestion(question) || isMultiSelectQuestion(question)) {
      children.push(
        new Paragraph({

          children:
            question.options.filter(option => !option.isFreeText).map(option => {
              return new TextRun({
                text: `☐ ${option.text[language]}\t`,
              });
            })
        })
      );

      if (question.options.some(option => option.isFreeText)) {
        const freeTextOption = question.options.find(option => option.isFreeText);

        children.push(
          new Paragraph({
            children: [
              new TextRun(`☐ ${freeTextOption?.text[language] ?? ""}`)
            ],
          })
        );

        // 2. Input line (separate row)
        children.push(
          new Paragraph({
            border: {
              bottom: {
                color: "000000",
                space: 1,
                style: BorderStyle.SINGLE,
                size: 1
              },
            },
            children: [new TextRun("")],
          })
        );
      }
    }

    if (isRatingQuestion(question)) {
      if (question.lowerLabel?.[language]) {
        children.push(new Paragraph({
          children: [
            new TextRun({ text: `1 - ${question.lowerLabel[language]} : `, italics: true })
          ]
        }));
      }

      if (question.upperLabel?.[language]) {
        children.push(new Paragraph({
          children: [
            new TextRun({ text: `${ratingScaleToNumber(question.scale)} - ${question.upperLabel[language]} : `, italics: true })
          ]
        }));
      }

      children.push(
        new Paragraph({
          spacing: {
            after: 100, // space between lines
          },
          children:
            Array.from({ length: ratingScaleToNumber(question.scale) }, (_, i) => i + 1).map(index => {
              return new TextRun({
                text: `☐ ${index}\t`,
              });
            })
        })
      );
    }

    return children;
  }

  private createHeading(text: string): Paragraph {
    return new Paragraph({
      text: text,
      heading: HeadingLevel.HEADING_1,
      thematicBreak: true
    });
  }
}
