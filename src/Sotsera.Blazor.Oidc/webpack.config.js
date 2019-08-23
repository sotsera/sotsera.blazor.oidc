// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

const fs = require('fs');
const xml2js = require('xml2js');
const path = require("path");
const webpack = require("webpack");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");

const banner = [
    "Copyright (c) Alessandro Ghidini. All rights reserved.",
    "Licensed under the Apache License, Version 2.0. See License.txt in the project root or http://www.apache.org/licenses/LICENSE-2.0 for license information.",
    "Based on https://github.com/IdentityModel/oidc-client-js by Brock Allen & Dominick Baier licensed under the Apache License, Version 2.0"
].join("\n");

const entry = {};
entry[`sotsera.blazor.oidc-${readProjectVersion()}`] = "./BrowserInterop/index.ts";

module.exports = (env, args) => ({
    mode: args.mode,
    resolve: { extensions: [".ts", ".js"] },
    devtool: args.mode === "development" ? "source-map" : "none",
    module: {
        rules: [{ test: /\.ts?$/, loader: "ts-loader" }]
    },
    entry: entry,
    output: { path: path.join(__dirname, "/wwwroot"), filename: "[name].js" },
    plugins: [
        new webpack.BannerPlugin({ banner: banner }),
        new CleanWebpackPlugin({
            cleanOnceBeforeBuildPatterns: ["sotsera.blazor.oidc-*.js*"]
        })
    ]
});


function readProjectVersion()
{
    const parser = new xml2js.Parser();
    const filePath = path.join(__dirname, "Sotsera.Blazor.Oidc.csproj");

    const content = fs.readFileSync(filePath, "utf-8");

    let result;

    parser.parseString(content, function (err, json) {
        if (err) throw Error(`Unable to read the project file content: ${err}`);
        result = json.Project.PropertyGroup[0].Version[0];
    });

    return result;
}