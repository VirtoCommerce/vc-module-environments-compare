angular.module('VirtoCommerce.EnvironmentsCompare')
    .factory('VirtoCommerce.EnvironmentsCompare.webApi', ['$resource', function ($resource) {
        return $resource('api/environments-compare');
    }]);
