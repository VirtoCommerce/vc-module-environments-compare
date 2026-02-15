angular.module('VirtoCommerce.EnvironmentsCompare')
    .directive('vcEnvironmentsCompareSearch', [function () {
        return {
            restrict: 'E',
            templateUrl: function (elem, attrs) {
                return attrs.templateUrl ||
                    'Modules/$(VirtoCommerce.EnvironmentsCompare)/Scripts/directives/environmentsCompareSearch.tpl.html';
            },
            scope: {
                blade: '='
            },
            link: function ($scope) {
                var blade = $scope.blade;
                var filter = $scope.filter = blade.filter;

                if (!filter) {
                    filter = $scope.filter = blade.filter = {
                        keyword: '',
                        criteriaChanged: function () { }
                    };
                } else {
                    filter.keyword = filter.keyword || '';
                    filter.criteriaChanged = filter.criteriaChanged || function () { };
                }

                filter.filterByKeyword = function () {
                    if (angular.isFunction(filter.criteriaChanged)) {
                        filter.criteriaChanged();
                    }
                };
            }
        };
    }]);


