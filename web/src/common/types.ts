export type FunctionComponent = React.ReactElement | null;

type HeroIconSVGProps = React.PropsWithoutRef<React.SVGProps<SVGSVGElement>> &
	React.RefAttributes<SVGSVGElement>;
type IconProps = HeroIconSVGProps & {
	title?: string;
	titleId?: string;
};
export type Heroicon = React.FC<IconProps>;

export type PageParameters = {
	pageNumber: number; // 1-based (the first page is 1)
	pageSize: number;
};

export type PageResponse<T> = {
	currentPage: number;
	pageSize: number;
	totalCount: number;
	items: T[];
};
