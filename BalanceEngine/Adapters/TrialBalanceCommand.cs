/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Command payload                         *
*  Type     : TrialBalanceCommand                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to build trial balances                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Command payload used to build trial balances.</summary>
  public class TrialBalanceCommand {


    public int TrialBalanceType {
      get; set;
    } = 1;
    public DateTime StartDate {
      get; set;
    } = DateTime.Today;


    public DateTime EndDate {
      get; set;
    } = DateTime.Today.AddDays(1);


    public string Fields {
      get; set;
    } = string.Empty;


    public string Grouping {
      get; set;
    } = string.Empty;

    public string Having {
      get; set;
    } = string.Empty;


    public string Ordering {
      get; set;
    } = string.Empty;


    public string[] Sectors {
      get; set;
    } = new string[0];


    public string FromAccount {
      get; set;
    } = string.Empty;


    public string ToAccount {
      get; set;
    } = string.Empty;



    public int LedgerId {
      get; set;
    } = -1;


    public int AccountCatalogueId {
      get; set;
    } = -1;

    public int LedgerAccountId {
      get; set;
    } = -1;


    public int AccountId {
      get; set;
    } = -1;


    public int OutputFormat {
      get; set;
    } = 1;


  } // class TrialBalanceCommand


  /// <summary>Extension methods for TrialBalanceCommand interface adapter.</summary>
  static public class TrialBalanceCommandExtension {
    #region Public methods

    static public string MapToFieldString(this TrialBalanceCommand command) {
      string fields = String.Empty;

      command.TrialBalanceType = command.TrialBalanceType != -1 ? command.TrialBalanceType : 1;

      fields = BuildFieldTypesFilter(command.TrialBalanceType);

      return fields;
    }
 

    static public string MapToFilterString(this TrialBalanceCommand command) {
      //string startDateFilter = BuildBalanceStartDateFilter(command.StartDate);
      //string endDateFilter = BuildBalanceEndDateFilter(command.EndDate);
      string sectorFilter = BuildBalanceSectorFilter(command.Sectors);
      string rangeFilter = BuildBalanceRangeFilter(command.FromAccount, command.ToAccount);
      
      var filter = new Filter(sectorFilter);

      //filter.AppendAnd(endDateFilter);
      //filter.AppendAnd(sectorFilter);
      filter.AppendAnd(rangeFilter);

      return filter.ToString();
    }
    #endregion Public methods

    #region Private methods

    static private string BuildBalanceStartDateFilter(DateTime? startDate) {

      startDate = !startDate.HasValue ? DateTime.Today : startDate;

      string formattedStartDate = CommonMethods.FormatSqlDate(startDate.Value);
     
      return $"FECHA_AFECTACION >= '{formattedStartDate}'";
    }

    static private string BuildBalanceEndDateFilter(DateTime? endDate) {

      endDate = !endDate.HasValue ? DateTime.Today.AddDays(1) : endDate;

      string formattedEndDate = CommonMethods.FormatSqlDate(endDate.Value);

      return $"FECHA_AFECTACION <= '{formattedEndDate}'";
    }


    static private string BuildBalanceRangeFilter(string fromAccount, string toAccount) {
      string filter = String.Empty;

      if (fromAccount.Length != 0) {
        filter = $"NUMERO_CUENTA_ESTANDAR >= '{fromAccount}'";
      }
      if (toAccount.Length != 0) {
        if (filter.Length != 0) {
          filter += " AND ";
        }
        filter += $"NUMERO_CUENTA_ESTANDAR <= '{toAccount}'";
      }

      return filter;
    }


    static private string BuildBalanceSectorFilter(string[] sectors) {
      if (sectors.Length == 0) {
        return string.Empty;
      }

      string[] SectorIds = sectors.Select(x => $"'{x}'")
                                             .ToArray();
      
      return $"AND ID_SECTOR IN ({String.Join(", ", SectorIds)})";
    }

    static private string BuildFieldTypesFilter(int typeId) {
      string fields = String.Empty;

      if (typeId==1) {
        fields = "-1 AS ID_MAYOR, -1 AS ID_MONEDA, -1 AS ID_CUENTA, ID_CUENTA_ESTANDAR, " +
                 "'-1' AS NUMERO_CUENTA_ESTANDAR, ID_SECTOR, '-1' AS NOMBRE_CUENTA_ESTANDAR, " +
                 "SALDO_ANTERIOR, CARGOS, ABONOS, SALDO_ACTUAL ";
      }

      if (typeId == 2) {
        fields = "ID_MAYOR, ID_MONEDA, ID_CUENTA, ID_CUENTA_ESTANDAR, " +
                 "NUMERO_CUENTA_ESTANDAR, ID_SECTOR, NOMBRE_CUENTA_ESTANDAR, " +
                 "SALDO_ANTERIOR, CARGOS, ABONOS, SALDO_ACTUAL ";
      }
    
      return fields;
    }

    #endregion Private methods

  } // class TrialBalanceCommandExtension


} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
