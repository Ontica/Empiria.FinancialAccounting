/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : BalanzaDolarizadaMapper                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map balanza dolarizada.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Methods used to map balanza dolarizada.</summary>
  static internal class BalanzaDolarizadaMapper {


    #region Public methods

    #endregion Public methods

    static internal BalanzaDolarizadaDto Map(TrialBalanceQuery query,
                                             FixedList<BalanzaDolarizadaEntry> entries) {

      throw new NotImplementedException();
      
    }



    #region Private methods


    static public FixedList<DataTableColumn> DataColumns() {
      throw new NotImplementedException();
    }


    static public BalanzaDolarizadaEntryDto MapEntry(BalanzaDolarizadaEntry x) {
      throw new NotImplementedException();
    }


    #endregion Private methods

  } // class BalanzaDolarizadaMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
