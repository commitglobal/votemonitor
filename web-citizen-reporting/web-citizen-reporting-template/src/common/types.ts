export interface ElectionModel {
  id: string;
  title: string;
  englishTitle: string;
  startDate: string;
  country: string;
  slug: string;
  shortDescription: string;
}

export interface ElectionDetailsModel {
  title: string;
  englishTile: string;
  startDate: string;
  country: string;
}

export interface FeatureShape {
  type: "Feature";
  id: string;
  geometry: { coordinates: [number, number][][]; type: "Polygon" };
  properties: { name: string; a3: string };
}
