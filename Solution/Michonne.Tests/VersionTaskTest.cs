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
            Check.That(buildVersionId).IsEqualTo("0.4.0-nightly-054");
        }

        private static void BuildVersionID(string version, string stream, out string fullVersion)
        {
            var builder = new StringBuilder(version.Length+stream.Length);
            var parts = (version+".0.0").Split('.');
            parts[3] = int.Parse(parts[3]).ToString("000");
            builder.AppendFormat("{0}.{1}.{2}-{4}-{3}", parts[0], parts[1], parts[2], parts[3], stream);
            fullVersion=builder.ToString();
        }
    }
}
