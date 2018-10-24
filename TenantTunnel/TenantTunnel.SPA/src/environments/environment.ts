// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,

  backend_api: 'https://localhost:44303',
  adalConfig: {
    tenant: 'common',
    clientId: 'd5a2006e-48fa-44bb-b1c1-4fe3e57de967' // application id
  },
  adalConfigApiEndpoint: 'a5cb06b7-a3ee-4263-8cf9-3985da161906', // application id of backend
};
