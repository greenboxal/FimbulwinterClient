using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FimbulwinterClient.Utils;

namespace FimbulwinterClient.Network.Packets
{
    public class RejectLogin : InPacket
    {
        public int Result { get; set; }
        public string Text { get; set; }

        public override bool Read(byte[] data)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(data));

            Result = br.ReadByte();
            Text = br.ReadCString(20);

            if (Result != 6)
            {
                switch (Result)
                {
                    case 0: Text = "Unregistered ID."; break; // 0 = Unregistered ID
                    case 1: Text = "Incorrect Password."; break; // 1 = Incorrect Password
                    case 2: Text = "Account Expired."; break; // 2 = This ID is expired
                    case 3: Text = "Rejected from server."; break; // 3 = Rejected from Server
                    case 4: Text = "Blocked by GM."; break; // 4 = You have been blocked by the GM Team
                    case 5: Text = "Not latest game EXE."; break; // 5 = Your Game's EXE file is not the latest version
                    case 6: Text = "Banned."; break; // 6 = Your are Prohibited to log in until %s
                    case 7: Text = "Server Over-population."; break; // 7 = Server is jammed due to over populated
                    case 8: Text = "Account limit from company"; break; // 8 = No more accounts may be connected from this company
                    case 9: Text = "Ban by DBA"; break; // 9 = MSI_REFUSE_BAN_BY_DBA
                    case 10: Text = "Email not confirmed"; break; // 10 = MSI_REFUSE_EMAIL_NOT_CONFIRMED
                    case 11: Text = "Ban by GM"; break; // 11 = MSI_REFUSE_BAN_BY_GM
                    case 12: Text = "Working in DB"; break; // 12 = MSI_REFUSE_TEMP_BAN_FOR_DBWORK
                    case 13: Text = "Self Lock"; break; // 13 = MSI_REFUSE_SELF_LOCK
                    case 14: Text = "Not Permitted Group"; break; // 14 = MSI_REFUSE_NOT_PERMITTED_GROUP
                    case 15: Text = "Not Permitted Group"; break; // 15 = MSI_REFUSE_NOT_PERMITTED_GROUP
                    case 99: Text = "Account gone."; break; // 99 = This ID has been totally erased
                    case 100: Text = "Login info remains."; break; // 100 = Login information remains at %s
                    case 101: Text = "Hacking investigation."; break; // 101 = Account has been locked for a hacking investigation. Please contact the GM Team for more information
                    case 102: Text = "Bug investigation."; break; // 102 = This account has been temporarily prohibited from login due to a bug-related investigation
                    case 103: Text = "Deleting char."; break; // 103 = This character is being deleted. Login is temporarily unavailable for the time being
                    case 104: Text = "Deleting spouse char."; break; // 104 = This character is being deleted. Login is temporarily unavailable for the time being
                    default: Text = "Unknown Error."; break;
                }
            }

            return true;
        }
    }
}
