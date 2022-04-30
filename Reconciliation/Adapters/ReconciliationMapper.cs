/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Mapper class                            *
*  Type     : ReconciliationMapper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains static mapping methods for reconciliation processes' results.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Contains static mapping methods for reconciliation processes' results.</summary>
  static internal class ReconciliationMapper {


    static internal FixedList<ReconciliationTypeDto> Map(FixedList<ReconciliationType> list) {
      return new FixedList<ReconciliationTypeDto>(list.Select(x => Map(x)));
    }

    static internal ReconciliationResultDto Map(ReconciliationResult result) {
      return new ReconciliationResultDto {
        Command = result.Command,
        Columns = MapColumns(),
        Entries = new FixedList<ReconciliationResultEntryDto>()
      };
    }

    #region Private methods

    static private ReconciliationTypeDto Map(ReconciliationType x) {
      return new ReconciliationTypeDto() {
        UID = x.UID,
        Name = x.Name,
        ExportTo = ExportToMapper.Map(x.ExportTo)
      };
    }


    static private FixedList<DataTableColumn> MapColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("sectorCode", "Sector", "text"));
      columns.Add(new DataTableColumn("operationalTotal", "Mov Operativo", "decimal"));
      columns.Add(new DataTableColumn("accountingTotal", "Mov Contable", "decimal"));
      columns.Add(new DataTableColumn("difference", "Diferencia", "decimal"));

      return columns.ToFixedList();
    }


    #endregion Private methods

  }  // class ReconciliationMapper

}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters
