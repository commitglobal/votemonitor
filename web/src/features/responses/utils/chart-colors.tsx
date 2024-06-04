import { SelectOption } from "@/common/types";

export const purple500 = '#7A33B3';

export const baseColors = ['#7833B3', '#7A5FBC', '#7784C4', '#6DA8CC', '#57CCD3', '#FFF7C1', '#FFEEA0', '#FFE57C', '#FFDC53', '#FFD209'];
export const flaggedAnswersColors = ['#E23D3D', '#E55151', '#E86565', '#EB7979', '#EE8D8D', '#F1A1A1', '#F4B5B5', '#F7C9C9', '#FADDDD', '#FEF2F2'];

export function getChartColors(numberOfColors: number): string[] {
    const result: string[] = [];

    for (let index = 0; index < numberOfColors; index++) {
        result.push(baseColors[index % baseColors.length]!);
    }

    return result;
}

export function getColorsForSelectChart(options: SelectOption[]): string[] {
    const result: string[] = [];

    for (let index = 0; index < options.length; index++) {
        if (options[index]!.isFlagged) {
            result.push(flaggedAnswersColors[index % baseColors.length]!);
        } else {
            result.push(baseColors[index % baseColors.length]!);
        }
    }

    return result;
}