import { definePreset } from "@primeuix/themes";
import Aura from "@primeuix/themes/aura";

export const MelodiaPreset = definePreset(Aura, {
  semantic: {
    primary: {
      50: "#ecfdf5",
      100: "#d1fae8",
      200: "#a8f0d0",
      300: "#85e6bd",
      400: "#68dba9",
      500: "#3fc28c",
      600: "#2a9d71",
      700: "#217c5a",
      800: "#1a5a44",
      900: "#144838",
      950: "#0b2a20",
    },
    colorScheme: {
      dark: {
        surface: {
          0: "#ffffff",
          50: "#f1f4f8",
          100: "#e2e6ec",
          200: "#c6ced9",
          300: "#a9b4c2",
          400: "#8e9bae",
          500: "#5b6578",
          600: "#3a4152",
          700: "#222736",
          800: "#1a1f2c",
          900: "#12151d",
          950: "#0a0c10",
        },
        primary: {
          color: "{primary.400}",
          contrastColor: "#003825",
          hoverColor: "#85f8c4",
          activeColor: "{primary.300}",
        },
        formField: {
          background: "{surface.900}",
          borderColor: "{surface.700}",
        },
        overlay: {
          modal: {
            background: "{surface.800}",
          },
        },
      },
    },
  },
});
