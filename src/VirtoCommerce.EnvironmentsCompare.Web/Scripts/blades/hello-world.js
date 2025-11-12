angular.module('VirtoCommerce.EnvironmentsCompare')
    .controller('VirtoCommerce.EnvironmentsCompare.helloWorldController', ['$scope', 'VirtoCommerce.EnvironmentsCompare.webApi', function ($scope, api) {
        var blade = $scope.blade;
        blade.title = 'EnvironmentsCompare';

        blade.refresh = function () {
            api.get(function (data) {
                blade.title = 'EnvironmentsCompare.blades.hello-world.title';
                blade.data = data.result;
                blade.isLoading = false;
            });
        };

        blade.refresh();
    }]);
