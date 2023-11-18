/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : IntegracionSaldosCapitalBuilder               License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Generates report 'Integración de saldos de capital e intereses'.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;


using Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters;

namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses {

  /// <summary>Generates report 'Integración de saldos de capital e intereses'.</summary>
  internal class IntegracionSaldosCapitalBuilder : IReportBuilder {

    #region Public methods

    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      var baseBuilder = new IntegracionSaldosCapitalInteresesBuilder();

      List<IIntegracionSaldosCapitalInteresesEntry> data = baseBuilder.BuildEntries(buildQuery);

      data.RemoveAll(x => x is IntegracionSaldosCapitalInteresesSubTotal);

      return IntegracionSaldosCapitalMapper.MapToReportDataDto(buildQuery, data);
    }

    #endregion Public methods

  } // class IntegracionSaldosCapitalBuilder

} // namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses
