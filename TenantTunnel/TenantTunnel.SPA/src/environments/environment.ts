// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  backend_api: 'https://localhost:44379',
  
  adalConfig: {
    tenant: 'common',
    clientId: 'c1b96c7f-2776-43b1-9062-aa041298865a' // application id
  },
  adalConfigApiEndpoint: '8afade94-3388-4016-a2d2-6ac272fcd54d', // application id of backend
};
