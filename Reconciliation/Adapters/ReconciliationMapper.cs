/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Mapper class                            *
*  Type     : ReconciliationMapper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map reconciliation types and their data.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Methods used to map reconciliation types and their data.</summary>
  static internal class ReconciliationMapper {

    #region Public mappers

    //static internal FixedList<ReconciliationTypeDto> Map(FixedList<ReconciliationType> list) {
    //  var mapped = list.Select((x) => Map(x));

    //  return new FixedList<ReconciliationTypeDto>(mapped);
    //}


    #endregion Public mappers

    #region Private methods

    //static private ReconciliationTypeDto Map(ReconciliationType reconciliationType) {
    //  return new ReconciliationTypeDto {
    //    UID = reconciliationType.UID,
    //    Name = reconciliationType.Name
    //  };
    //}

    #endregion Private methods

  } // class ReconciliationMapper

} // Empiria.FinancialAccounting.Reconciliation.Adapters
