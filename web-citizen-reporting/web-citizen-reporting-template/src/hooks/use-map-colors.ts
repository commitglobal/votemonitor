import { useTheme } from "next-themes";
import React from "react";
import { defaultStyles } from "@visx/tooltip";

export interface WorldMapConfig {
  landColor: string;
  landStrokeColor: string;
  waterColor: string;
  coveredCountryColor: string;
  coveredCountryHooverColor: string;
}

const lightThemeColors: WorldMapConfig = {
  landColor: "#F8F8F8", // Softer, near-white for a modern look
  landStrokeColor: "#C2C2C2", // Slightly darker gray for better definition
  waterColor: "#E0EAF5", // Light blue-gray for a more natural water tone
  coveredCountryColor: "#F7A600", // A warmer golden yellow for better contrast
  coveredCountryHooverColor: "#FFD54F", // A slightly brighter hover effect
};

const darkThemeColors: WorldMapConfig = {
  landColor: "rgba(51, 51, 51, 1)",
  landStrokeColor: "rgba(43, 43, 43, 1)",
  waterColor: "rgba(43, 43, 43, 1)",
  coveredCountryColor: "rgba(244, 227, 178, 1)",
  coveredCountryHooverColor: "rgba(232, 201, 121, 1)",
};

export interface MapColorsConfig {
  worldMapColors: WorldMapConfig;
  tooltipStyles: React.CSSProperties;
}

export function useMapColors(): MapColorsConfig {
  const { resolvedTheme } = useTheme();

  const worldMapColors = React.useMemo(() => {
    return resolvedTheme !== "dark"
      ? {
          ...lightThemeColors,
        }
      : {
          ...darkThemeColors,
        };
  }, [resolvedTheme]);

  const tooltipStyles = React.useMemo(() => {
    return resolvedTheme !== "dark"
      ? {
          ...defaultStyles,
          background: "#f9f9f9",
          border: `1px solid #353535`,
          borderRadius: "0.25rem",
          color: "#030303",
          fontSize: "1.2rem",
        }
      : {
          ...defaultStyles,
          background: "#080808",
          border: `1px solid #353535`,
          borderRadius: "0.25rem",
          color: "#c2c2c2",
          fontSize: "1.2rem",
        };
  }, [resolvedTheme]);

  return {
    worldMapColors,
    tooltipStyles,
  };
}
