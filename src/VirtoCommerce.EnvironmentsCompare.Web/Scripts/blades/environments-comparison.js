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
                const blade = $scope.blade;

                blade.title = 'environments-compare.blades.environments-comparison.title';
                blade.nameColWidth = 500;
                blade.valueColWidth = 250;
                blade.totalWidth = blade.nameColWidth + blade.valueColWidth * blade.environmentNames.length;

                blade.refresh = function () {
                    environmentsCompareApi.compareEnvironments(
                        { environmentNames: blade.environmentNames, baseEnvironmentName: blade.baseEnvironmentName },
                        function (compareEnvironmentsResult) {
                            blade.data = compareEnvironmentsResult;
                            blade.isLoading = false;
                        });
                };

                blade.getComparedValue = function (settings, environmentName) {
                    const item = settings.comparedValues.filter(x => x.environmentName === environmentName)[0];
                    return item.value;
                }

                blade.hasDiff = function (settings, environmentName) {
                    const item = settings.comparedValues.filter(x => x.environmentName === environmentName)[0];
                    return !item.errorMessage && !item.equalsBaseValue;
                }

                blade.hasAnyDiff = function (settings) {
                    return settings.comparedValues.filter(x => !x.errorMessage && !x.equalsBaseValue).length > 0;
                }

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
