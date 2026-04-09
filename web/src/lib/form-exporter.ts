import { isDateQuestion, isMultiSelectQuestion, isNumberQuestion, isRatingQuestion, isSingleSelectQuestion, isTextQuestion } from "@/common/guards";
import { BaseQuestion, QuestionType } from "@/common/types";
import { FormTemplateFull } from "@/features/form-templates/models";
import { FormFull } from "@/features/forms/models";
import {
  AlignmentType,
  BorderStyle,
  Document,
  FileChild,
  HeadingLevel,
  Packer,
  Paragraph,
  Table,
  TableCell,
  TableOfContents,
  TableRow,
  TabStopPosition,
  TabStopType,
  TextRun,
  UnderlineType,
  WidthType
} from "docx";
import { ratingScaleToNumber } from "./utils";

export class FormExporter {
  // tslint:disable-next-line: typedef
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
        heading: HeadingLevel.HEADING_1
      }),
      ...this.createObservationSection(),
      this.createHeading("Questions"),
      ...form.questions.flatMap(question => this.createQuestionSection(question, language)),
    ];
  }



  public createObservationSection(): FileChild[] {
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
              size: 6,
              color: "000000",
            },
          },
          children: [new Paragraph("")],
        }),
      ],
    });
  }


  public createQuestionSection(question: BaseQuestion, language: string): FileChild[] {
    const children: FileChild[] = [];

    children.push(new Paragraph({
      children: [
        new TextRun(
          { text: `${question.code}. `, bold: true }
        ),
        new TextRun({
          text: question.text[language],
        })
      ]
    }));

    if (question.helptext?.[language]) {
      children.push(new Paragraph({
        children: [
          new TextRun({ text: question.helptext[language], italics: true })
        ]
      }));
    }

    if (isTextQuestion(question)) {
      for (let i = 0; i < 3; i++) {
        children.push(
          new Paragraph({
            border: {
              bottom: {
                color: "000000",
                space: 1,
                style: BorderStyle.SINGLE,
                size: 6,
              },
            },
            spacing: {
              after: 200, // space between lines
            },
            children: [new TextRun("")],
          })
        );
      }
    }
    if (isNumberQuestion(question)) {
      children.push(
        new Paragraph({
          border: {
            bottom: {
              color: "000000",
              space: 1,
              style: BorderStyle.SINGLE,
              size: 6,
            },
          },
          spacing: {
            after: 200, // space between lines
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
              size: 6,
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
          spacing: {
            after: 100, // space between lines
          },
          children:
            question.options.filter(option => !option.isFreeText).map(option => {
              return new TextRun({
                text: `☐ ${option.text[language]}\t`,
              });
            })
        })
      );

      if (question.options.some(option => option.isFreeText)) {
        children.push(
          new Paragraph({
            border: {
              bottom: {
                color: "000000",
                space: 1,
                style: BorderStyle.SINGLE,
                size: 6,
              },
            },
            spacing: {
              after: 200, // space between lines
            },
            children: [new TextRun(`☐ ${question.options.find(option => option.isFreeText)?.text[language] ?? ""} \n`)],
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

  public createHeading(text: string): Paragraph {
    return new Paragraph({
      text: text,
      heading: HeadingLevel.HEADING_1,
      thematicBreak: true
    });
  }

  public createSubHeading(text: string): Paragraph {
    return new Paragraph({
      text: text,
      heading: HeadingLevel.HEADING_2
    });
  }

  public createInstitutionHeader(
    institutionName: string,
    dateText: string
  ): Paragraph {
    return new Paragraph({
      tabStops: [
        {
          type: TabStopType.RIGHT,
          position: TabStopPosition.MAX
        }
      ],
      children: [
        new TextRun({
          text: institutionName,
          bold: true
        }),
        new TextRun({
          text: `\t${dateText}`,
          bold: true,
          underline: {
            type: UnderlineType.DASH
          }
        })
      ]
    });
  }

  public createRoleText(roleText: string): Paragraph {
    return new Paragraph({
      children: [
        new TextRun({
          text: roleText,
          italics: true
        })
      ]
    });
  }

  public createBullet(text: string): Paragraph {
    return new Paragraph({
      text: text,
      bullet: {
        level: 0
      }
    });
  }

  // tslint:disable-next-line:no-any
  public createSkillList(skills: any[]): Paragraph {
    return new Paragraph({
      children: [new TextRun(skills.map(skill => skill.name).join(", ") + ".")]
    });
  }

  // tslint:disable-next-line:no-any
  public createAchivementsList(achivements: any[]): Paragraph[] {
    return achivements.map(
      achievement =>
        new Paragraph({
          text: achievement.name,
          bullet: {
            level: 0
          }
        })
    );
  }

  public createInterests(interests: string): Paragraph {
    return new Paragraph({
      children: [new TextRun(interests)]
    });
  }

  public splitParagraphIntoBullets(text: string): string[] {
    return text.split("\n\n");
  }

  // tslint:disable-next-line:no-any
  public createPositionDateText(
    startDate: any,
    endDate: any,
    isCurrent: boolean
  ): string {
    const startDateText =
      this.getMonthFromInt(startDate.month) + ". " + startDate.year;
    const endDateText = isCurrent
      ? "Present"
      : `${this.getMonthFromInt(endDate.month)}. ${endDate.year}`;

    return `${startDateText} - ${endDateText}`;
  }

  public getMonthFromInt(value: number): string {
    switch (value) {
      case 1:
        return "Jan";
      case 2:
        return "Feb";
      case 3:
        return "Mar";
      case 4:
        return "Apr";
      case 5:
        return "May";
      case 6:
        return "Jun";
      case 7:
        return "Jul";
      case 8:
        return "Aug";
      case 9:
        return "Sept";
      case 10:
        return "Oct";
      case 11:
        return "Nov";
      case 12:
        return "Dec";
      default:
        return "N/A";
    }
  }
}
