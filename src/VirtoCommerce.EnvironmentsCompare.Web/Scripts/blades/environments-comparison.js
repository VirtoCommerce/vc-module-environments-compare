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
                blade.markerColWidth = 5;
                blade.nameColWidth = 500;
                blade.valueColWidth = 250;
                blade.totalWidth = blade.markerColWidth + blade.nameColWidth + blade.valueColWidth * blade.environmentNames.length;

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

                blade.environmentComparedValueHasDiff = function (settings, environmentName) {
                    const comparedValue = settings.comparedValues.filter(x => x.environmentName === environmentName)[0];
                    return blade.comparedValueHasDiff(comparedValue);
                }

                blade.environmentComparedValueHasError = function (settings, environmentName) {
                    const comparedValue = settings.comparedValues.filter(x => x.environmentName === environmentName)[0];
                    return blade.comparedValueHasError(comparedValue);
                }

                blade.anyEnvironmentComparedValueHasDiff = function (settings) {
                    return settings.comparedValues.filter(x => blade.comparedValueHasDiff(x)).length > 0;
                }

                blade.comparedValueHasDiff = function (item) {
                    return item.equalsBaseValue === false;
                }

                blade.comparedValueHasError = function (item) {
                    return !!item.errorMessage;
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
