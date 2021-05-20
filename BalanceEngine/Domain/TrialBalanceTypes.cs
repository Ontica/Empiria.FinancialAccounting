/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria General Object                  *
*  Type     : TrialBalanceTypes                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the order of fields depending on the trialBalanceType.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains the order of fields depending on the trialBalanceType.</summary>
  internal class TrialBalanceTypes {

    #region Public methods
    
    static public string[] MapToFieldString(int trialBalanceType) {
      string[] fieldsAndGrouping = new string[2];

      trialBalanceType = trialBalanceType != -1 ? trialBalanceType : 1;

      fieldsAndGrouping = BuildFieldTypes(trialBalanceType);

      return fieldsAndGrouping;
    }

    #endregion

    #region Private methods
    
    static private string[] BuildFieldTypes(int type) {
      string[] fieldsGrouping = new string[2];
      string fields = String.Empty;
      string grouping = String.Empty;

      switch (type) {
        case 1:
          fields = "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, " +
                   "SALDO_ANTERIOR, CARGOS, ABONOS, SALDO_ACTUAL";

          grouping = "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, " +
                     "SALDO_ANTERIOR, CARGOS, ABONOS, SALDO_ACTUAL";
          break;

        case 2:
          fields = "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, " +
                 "NUMERO_CUENTA_ESTANDAR, ID_SECTOR, NOMBRE_CUENTA_ESTANDAR, " +
                 "SALDO_ANTERIOR, CARGOS, ABONOS, SALDO_ACTUAL ";

          grouping = "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, " +
                     "NUMERO_CUENTA_ESTANDAR, ID_SECTOR, NOMBRE_CUENTA_ESTANDAR, " +
                     "SALDO_ANTERIOR, CARGOS, ABONOS, SALDO_ACTUAL ";
          break;

        default:
          fields = String.Empty;
          grouping = String.Empty;
          break;
      }


      fieldsGrouping[0] = fields;
      fieldsGrouping[1] = grouping;

      return fieldsGrouping;
    }

    #endregion


  } // class TrialBalanceTypes

} // namespace Empiria.FinancialAccounting.BalanceEngine
