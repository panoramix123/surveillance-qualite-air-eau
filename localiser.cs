using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            string street = textBox1.Text;
            string ville = textBox2.Text; 
            string région = textBox3.Text;
            string code_postal = textBox4.Text;
            try
            {
                StringBuilder queryadresse = new StringBuilder(" ");
                queryadresse.Append(""); 
                if (street! =string.Empty)
            }
                queryadresse.Append(street "," + "+");
        }
          if (ville! =string.Empty)
            }
              queryadresse.Append(street "," + "+"); 
        }
           if (région! = string.Empty)
            }
                queryadresse.Append(street "," + "+"); 
        } 
            if (code_postal! = string.Empty)
            }
                queryadresse.Append(street "," + "+"); 
        }
    }

}
