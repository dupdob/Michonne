using System.Text;

namespace Michonne.Tests
{
    using NFluent;
    using NUnit.Framework;

    class VersionTaskTest
    {
        [Test]
        public void Should_Build_Properly_Formated_Version()
        {
            string buildVersionId;
            BuildVersionID("0.4.0.54", "nightly", out buildVersionId);
            Check.That(buildVersionId).IsEqualTo("0.4.0-nightly-54");
            BuildVersionID("0.4.0.154", "nightly", out buildVersionId);
            Check.That(buildVersionId).IsEqualTo("0.4.0-nightly-154");
            BuildVersionID("0.4.0.1542", "nightly", out buildVersionId);
            Check.That(buildVersionId).IsEqualTo("0.4.0-nightly-01542");
        }

        private static void BuildVersionID(string version, string stream, out string fullVersion)
        {
            if (stream == "")
            {
                fullVersion = version;
                return;
            }
            var builder = new StringBuilder(version.Length+stream.Length);
            var parts = (version+".0.0").Split('.');
            var buildId = int.Parse(parts[3]);
            if (buildId < 100)
            {
                parts[3] = buildId.ToString("00");
            }
            else if (buildId < 1000)
            {
                parts[3] = buildId.ToString("000");
            }
            else
            {
                parts[3] = buildId.ToString("00000");
            }
            builder.AppendFormat("{0}.{1}.{2}-{4}-{3}", parts[0], parts[1], parts[2], parts[3], stream);
            fullVersion=builder.ToString();
        }
    }
}
