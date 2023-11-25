/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : ActivoFijoDepreciacionBuilder                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Generates report 'Activo fijo depreciación'.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FixedAssetsDepreciation;

using Empiria.FinancialAccounting.Reporting.ActivoFijoDepreciacion.Adapters;

namespace Empiria.FinancialAccounting.Reporting.ActivoFijoDepreciacion {

  /// <summary>Generates report 'Activo fijo depreciación'.</summary>
  internal class ActivoFijoDepreciacionBuilder : IReportBuilder {

    #region Public methods

    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      var builder = new FixedAssetsDepreciationBuilder(buildQuery.ToDate, buildQuery.Ledgers);

      FixedList<FixedAssetsDepreciationEntry> data = builder.Build();

      return ActivoFijoDepreciacionMapper.MapToReportDataDto(buildQuery, data);
    }

    #endregion Public methods

  } // class ActivoFijoDepreciacionBuilder

} // namespace Empiria.FinancialAccounting.Reporting.ActivoFijoDepreciacion
