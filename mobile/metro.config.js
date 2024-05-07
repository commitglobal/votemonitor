// const { getDefaultConfig } = require("expo/metro-config");
const { getSentryExpoConfig } = require('@sentry/react-native/metro');


module.exports = (() => {
  // const config = getDefaultConfig(__dirname);
  const config = getSentryExpoConfig(__dirname);

  const { transformer, resolver } = config;

  config.transformer = {
    ...transformer,
    babelTransformerPath: require.resolve("react-native-svg-transformer"),
  };

  // https://docs.expo.dev/guides/minify/#remove-console-logs
  config.transformer.minifierConfig = {
    compress: {
      // The option below removes all console logs statements in production.
      drop_console: true,
    },
  };
  config.resolver = {
    ...resolver,
    assetExts: resolver.assetExts.filter((ext) => ext !== "svg"),
    sourceExts: [...resolver.sourceExts, "svg"],
  };

  return config;
})();
