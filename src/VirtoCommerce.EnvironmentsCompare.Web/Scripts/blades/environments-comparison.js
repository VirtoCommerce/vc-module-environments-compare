angular.module('VirtoCommerce.EnvironmentsCompare')
    .controller('VirtoCommerce.EnvironmentsCompare.environmentsComparisonController',
        [
            '$scope',
            'VirtoCommerce.EnvironmentsCompare.webApi',
            'platformWebApp.uiGridHelper',
            function (
                $scope,
                environmentsCompareApi,
                uiGridHelper) {
                var blade = $scope.blade;
                blade.title = 'environments-compare.blades.environments-comparison.title';

                blade.refresh = function () {
                    environmentsCompareApi.compareEnvironments(
                        { environmentNames: blade.environmentNames, baseEnvironmentName: blade.baseEnvironmentName },
                        function (compareEnvironmentsResult) {
                            blade.data = compareEnvironmentsResult;
                            blade.isLoading = false;
                        });
                };

                $scope.setGridOptions = function (gridOptions) {
                    uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                        $scope.gridApi = gridApi;
                    });
                };

                function initializeToolbar() {
                    blade.toolbarCommands = [
                        {
                            name: 'platform.commands.refresh',
                            icon: 'fa fa-refresh',
                            executeMethod: blade.refresh,
                            canExecuteMethod: function () {
                                return true;
                            }
                        }
                    ];
                }

                blade.refresh();
                initializeToolbar();
            }]);
