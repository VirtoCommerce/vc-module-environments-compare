angular.module('VirtoCommerce.EnvironmentsCompare')
    .factory('VirtoCommerce.EnvironmentsCompare.webApi', ['$resource', function ($resource) {
        return $resource('api/environments-compare', {}, {
            getEnvironments: { method: 'GET', url: 'api/environments-compare/get-environments', isArray: true },
            compareEnvironments: { method: 'POST', url: 'api/environments-compare/compare-environments' },
            getEnvironmentSettings: { method: 'GET', url: 'api/environments-compare/get-environment-settings/:environmentName' },
        });
    }]);
