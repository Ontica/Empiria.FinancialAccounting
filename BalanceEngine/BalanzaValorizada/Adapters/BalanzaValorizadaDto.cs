/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : BalanzaComparativaDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a Balanza valorizada.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Newtonsoft.Json;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Output DTO used to return a Balanza valorizada.</summary>
  public class BalanzaValorizadaDto {


    [JsonProperty]
    public TrialBalanceQuery Query {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<BalanzaValorizadaEntryDto> Entries {
      get; internal set;
    }


  } // class BalanzaValorizadaDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
