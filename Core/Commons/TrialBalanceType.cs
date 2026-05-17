/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service provider                        *
*  Type     : TrialBalanceEngine                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate a trial balance.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BalanceEngine {

  public enum TrialBalanceType {

    AnaliticoDeCuentas,

    Balanza,

    BalanzaConContabilidadesEnCascada,

    BalanzaEnColumnasPorMoneda,

    BalanzaDolarizada,

    BalanzaDiferenciaDiariaPorMoneda,

    BalanzaValorizadaComparativa,

    GeneracionDeSaldos,

    ResumenAjusteAnual,

    SaldosPorAuxiliar,

    SaldosPorAuxiliarConsultaRapida,

    SaldosPorAuxiliarID,

    SaldosPorCuenta,

    SaldosPorCuentaConsultaRapida,

    ValorizacionEstimacionPreventiva,

    /// Reclassificated Trial balances

    BalanzaAnaliticaOperaciones,

    BalanzaMonedaOrigenValorizada,

    BalanzaValorizada,

  }

  static public class TrialBalanceTypeExtensions {

    static public bool IsForReclassification(this TrialBalanceType trialBalanceType) {

      switch (trialBalanceType) {
        case TrialBalanceType.BalanzaAnaliticaOperaciones:
        case TrialBalanceType.BalanzaValorizada:
        case TrialBalanceType.BalanzaMonedaOrigenValorizada:
          return true;

        default:
          return false;
      }
    }
  }


  public enum BalancesType {

    AllAccounts,

    AllAccountsInCatalog,

    WithCurrentBalance,

    WithCurrentBalanceOrMovements,

    WithMovements

  }


  public enum TrialBalanceItemType {

    Entry,

    Summary,

    Group,

    Total,

    BalanceTotalGroupDebtor,

    BalanceTotalGroupCreditor,

    BalanceTotalDebtor,

    BalanceTotalCreditor,

    BalanceTotalCurrency,

    BalanceTotalConsolidatedByLedger,

    BalanceTotalConsolidated

  }


  public enum FileReportVersion {

    V1,

    V2
  }

} // namespace Empiria.FinancialAccounting.BalanceEngine
