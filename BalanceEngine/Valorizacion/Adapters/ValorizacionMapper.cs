/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : ValorizacionMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map valorized report.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map valorized report.</summary>
  static internal class ValorizacionMapper {


    #region Public methods

    static internal ValorizacionDto Map(TrialBalanceQuery query,
                                             FixedList<ValorizacionEntry> entries) {

      return new ValorizacionDto {
        Query = query,
        Columns = DataColumns(),
        Entries = entries.Select(x => MapEntry(x))
                         .ToFixedList()
      };
    }


    public static FixedList<DataTableColumn> DataColumns() {
      throw new NotImplementedException();
    }


    public static ValorizacionEntryDto MapEntry(ValorizacionEntry x) {
      throw new NotImplementedException();
    }


    #endregion Public methods


  } // class ValorizacionMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
