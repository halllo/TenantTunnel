// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  backend_api: 'https://localhost:44303',
  
  adalConfig: {
    tenant: 'common',
    clientId: '8dc5acea-27b9-4a50-b731-3d23d4c1a63b' // application id
  },
  adalConfigApiEndpoint: '9ce9c8ac-4c70-42f6-8738-1183899d4960', // application id of backend
};
