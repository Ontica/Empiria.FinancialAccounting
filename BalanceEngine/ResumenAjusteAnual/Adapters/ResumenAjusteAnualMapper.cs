/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : BalanzaDolarizadaMapper                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map resumen ajuste anual.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map resumen ajuste anual</summary>
  static internal class ResumenAjusteAnualMapper {

    #region Public methods

    static internal ResumenAjusteAnualDto Map(TrialBalanceQuery query,
                                             FixedList<ResumenAjusteAnualEntry> entries) {

      return new ResumenAjusteAnualDto {
        Query = query,
        Columns = DataColumns(),
        Entries = entries.Select(x => MapEntry(x))
                         .ToFixedList()
      };
    }


    static public FixedList<DataTableColumn> DataColumns() {

      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("parameter", "name", "text-nowrap"));
      columns.Add(new DataTableColumn("parameter", "name", "text"));
      columns.Add(new DataTableColumn("exchangeRate", "Tipo cambio", "decimal", 6));
      columns.Add(new DataTableColumn("parameter", "name", "decimal"));
      columns.Add(new DataTableColumn("date", "name", "date"));
      return columns.ToFixedList();
    }


    static public ResumenAjusteAnualEntryDto MapEntry(ResumenAjusteAnualEntry x) {

      return new ResumenAjusteAnualEntryDto {


      };
    }


    #endregion Public methods


    #region Private methods



    #endregion Private methods

  } // class ResumenAjusteAnualMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
