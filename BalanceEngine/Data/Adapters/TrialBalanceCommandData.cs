using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empiria.FinancialAccounting.BalanceEngine {
  internal class TrialBalanceCommandData {

    public DateTime InitialDate {
      get; set;
    } = DateTime.Now;


    public DateTime FinalDate {
      get; set;
    } = DateTime.Now.AddDays(1);


    public int BalanceGroupId {
      get; set;
    } = 1;


    public string Fields {
      get; set;
    } = string.Empty;


    public string Condition {
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

  }
}
