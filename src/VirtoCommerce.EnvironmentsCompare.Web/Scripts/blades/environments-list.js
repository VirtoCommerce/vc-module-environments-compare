angular.module('VirtoCommerce.EnvironmentsCompare')
    .controller('VirtoCommerce.EnvironmentsCompare.environmentsListController',
        [
            '$scope',
            'VirtoCommerce.EnvironmentsCompare.webApi',
            'platformWebApp.uiGridHelper',
            function (
                $scope,
                environmentsCompareApi,
                uiGridHelper) {
                var blade = $scope.blade;
                blade.title = 'environments-compare.blades.environments-list.title';

                blade.refresh = function () {
                    environmentsCompareApi.getEnvironments(function (data) {
                        blade.data = data;
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
                        },
                        {
                            name: 'platform.commands.compare',
                            icon: 'fas fa-not-equal',
                            executeMethod: $scope.add,
                            canExecuteMethod: hasTwoOrMoreSelectedItems,
                        },
                    ];
                }

                function hasTwoOrMoreSelectedItems() {
                    return $scope.gridApi?.selection?.getSelectedRows()?.length >= 2;
                }

                blade.refresh();
                initializeToolbar();
            }]);
