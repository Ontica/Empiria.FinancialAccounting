/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : BalanzaContabilidadesCascadaEntriesTests   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for 'Balanza con contabilidades en cascada' report entries.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using Empiria.Collections;
using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaContabilidadesCascada {

  /// <summary>Unit test cases for 'Balanza con contabilidades en cascada' report entries.</summary>
  public class BalanzaContabilidadesCascadaEntriesTests {

    private readonly EmpiriaHashTable<BalanzaContabilidadesCascadaDto> _cache =
                                        new EmpiriaHashTable<BalanzaContabilidadesCascadaDto>(16);

    #region Initialization

    public BalanzaContabilidadesCascadaEntriesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    

    #endregion Theories

  } // class BalanzaContabilidadesCascadaEntriesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaContabilidadesCascada
