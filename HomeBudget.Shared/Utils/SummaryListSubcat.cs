using HomeBudget.Code.Logic;
using System;
using System.Threading.Tasks;


namespace HomeBudget.Pages.Utils
{
    public class SummaryListSubcat
    {
        public double SpendPercentage
        {
            get
            {
                if(SubcatPlan != null && SubcatReal != null)
                {
                    return SubcatReal.Value == 0 ? 0 :
                        SubcatPlan.Value > 0 ? Math.Min((SubcatReal.Value / SubcatPlan.Value), 1) : 1;
                }
                else
                {
                    return 0;
                }
            }
         }
        public int SpendPercentageInt
        {
            get
            {
                return (int)(SpendPercentage * 100);
            }
        }
        public string Name { get; set; }
        public RealSubcat SubcatReal { get; set; }
        public PlannedSubcat SubcatPlan { get; set; }
        public int Id { get; set; }
        public string Icon { get; set; }

        public Action Expand;
        public Action Collapse;
        private TaskCompletionSource _tcs;

        public Task WaitForAddToList()
        {
            if(Expand != null && Collapse != null)
            {
                return Task.CompletedTask;
            }
            _tcs?.TrySetCanceled();
            _tcs = new TaskCompletionSource();
            return _tcs.Task;
        }

        public void MarkAddedToList()
        {
            _tcs.TrySetResult();
        }
    }
}
