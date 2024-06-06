import { SelectOption } from "@/common/types";

export const purple500 = '#7A33B3';

export const baseColors = [
    (opacity: number) => `rgba(120, 51, 179, ${opacity})`,
    (opacity: number) => `rgba(122, 95, 188, ${opacity})`,
    (opacity: number) => `rgba(119, 132, 196, ${opacity})`,
    (opacity: number) => `rgba(109, 168, 204, ${opacity})`,
    (opacity: number) => `rgba(87, 204, 211, ${opacity})`,
    (opacity: number) => `rgba(255, 247, 193, ${opacity})`,
    (opacity: number) => `rgba(255, 238, 160, ${opacity})`,
    (opacity: number) => `rgba(255, 229, 124, ${opacity})`,
    (opacity: number) => `rgba(255, 220, 83, ${opacity})`,
    (opacity: number) => `rgba(255, 210, 9, ${opacity})`
];

export const flaggedAnswersColors = [
    (opacity: number) => `rgba(226, 61, 61, ${opacity})`,
    (opacity: number) => `rgba(229, 81, 81, ${opacity})`,
    (opacity: number) => `rgba(232, 101, 101, ${opacity})`,
    (opacity: number) => `rgba(235, 121, 121, ${opacity})`,
    (opacity: number) => `rgba(238, 141, 141, ${opacity})`,
    (opacity: number) => `rgba(241, 161, 161, ${opacity})`,
    (opacity: number) => `rgba(244, 181, 181, ${opacity})`,
    (opacity: number) => `rgba(247, 201, 201, ${opacity})`,
    (opacity: number) => `rgba(250, 221, 221, ${opacity})`,
    (opacity: number) => `rgba(254, 242, 242, ${opacity})`
];
export function getChartColors(numberOfColors: number, forHoover: boolean = false): string[] {
    const result: string[] = [];

    for (let index = 0; index < numberOfColors; index++) {
        result.push(baseColors[index % baseColors.length]!(forHoover ? 0.8 : 1));
    }

    return result;
}

export function getColorsForSelectChart(options: SelectOption[], forHoover: boolean = false): string[] {
    const result: string[] = [];

    for (let index = 0; index < options.length; index++) {
        if (options[index]!.isFlagged) {
            result.push(flaggedAnswersColors[index % baseColors.length]!(forHoover ? 0.8 : 1));
        } else {
            result.push(baseColors[index % baseColors.length]!(forHoover ? 0.8 : 1));
        }
    }

    return result;
}