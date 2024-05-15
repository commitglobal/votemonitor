import { SelectOption } from "@/common/types";

export const purple500 = '#7A33B3';

export const purpleColors = [
    '#663366',
    '#a379bb',
    '#5aa2ae',
    '#9979b7',
    '#94618e',
    '#755d79',
    '#6a5b7e',
    '#593160',
    '#5d527f',
    '#4b466a'
];

export const yellowColors = [
    '#d2cb6c',
    '#fcb11c',
    '#ffc000',
    '#f3d13a',
    '#f2e03d',
    '#e6b91e',
    '#f7c229',
    '#f3cd35',
    '#e4d659',
    '#c8b144'
];

export const greenColors = [
    '#6bb1c9',
    '#50c49f',
    '#4baf73',
    '#6aad7e',
    '#3b9e77',
    '#34947d',
    '#3b8a5f',
    '#438d67',
    '#487a57',
    '#5c9c78'
];

export const blueColors = [
    '#2683c6',
    '#08a1d9',
    '#1b587c',
    '#0f6fc6',
    '#2c7c9f',
    '#0d335e',
    '#2f547a',
    '#327191',
    '#0a3e5c',
    '#265c86'
];

export const tealColors = [
    '#50c49f',
    '#3e8853',
    '#69ffff',
    '#31b6fd',
    '#44c1a3',
    '#37a76f',
    '#7ec251',
    '#80b606',
    '#86ce24',
    '#32c7a9',
];

export const redColors = [
    '#cf543f',
    '#f81b02',
    '#990000',
    '#cc0000',
    '#d31712',
    '#c01d1d',
    '#bf1b1b',
    '#b30808',
    '#a91d19',
    '#c31818',
];


export const colors = [...redColors,]

export function getChartColors(numberOfColors: number): string[] {
    const result: string[] = [];

    let availableColors = [
        purpleColors,
        yellowColors,
        greenColors,
        blueColors,
        tealColors,
    ];

    for (let index = 0; index < numberOfColors; index++) {
        const colorPalette = availableColors.shift()!;
        const color = colorPalette.shift()!;

        result.push(color);
        availableColors = [...availableColors, [...colorPalette, color]];
    }

    return result;
}

export function getColorsForSelectChart(options: SelectOption[]): string[] {
    const result: string[] = [];

    let availableColors = [
        purpleColors,
        yellowColors,
        greenColors,
        blueColors,
        tealColors
    ];

    let reds = [...redColors];

    for (let index = 0; index < options.length; index++) {
        if (options[index]!.isFlagged) {
            const redColor = reds.shift()!;
            result.push(redColor);
            reds = [...reds, redColor];
        } else {
            const colorPalette = availableColors.shift()!;
            const color = colorPalette.shift()!;

            result.push(color);
            availableColors = [...availableColors, [...colorPalette, color]];
        }
    }

    return result;
}