// lets node --test load app modules that import each other without the .ts extension (as Metro does)
import { register } from "node:module";

register("./resolve-ts.mjs", import.meta.url);
