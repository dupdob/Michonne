namespace Michonne.Sources.Tests
{
    using Michonne.Implementation;

    using NFluent;

    using NUnit.Framework;

    public class SteppingUnitTests
    {
        [Test]
        public void should_step_through_taks()
        {
            StepperUnit unit = new StepperUnit();
            int test = 0;

            unit.Dispatch(() => test = 1);
            unit.Dispatch(() => test =2);

            Check.That(test).IsEqualTo(0);
            unit.Step();
            Check.That(test).IsEqualTo(1);
            unit.Step();
            Check.That(test).IsEqualTo(2);
        }
    }
}
