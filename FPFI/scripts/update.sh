#!/bin/bash
update() {
	sudo rsync -iauvzP --exclude 'bin' --exclude 'obj' --exclude 'Migrations' $PATH2FPFI root@$SERVER:$PATH2SERVER
	ssh $SERVER << EOF
		cd $PATH2SERVER/FPFI/
		dotnet restore
		rm -R $PATH2SERVER/FPFI/bin/Release/netcoreapp2.1/linux-x64/
		dotnet publish -r linux-x64 -c Release
		systemctl stop kestrel-fpfi.service
		rm -R /var/fpfi/[A-Za-z]*
		rsync -iauvzP $PATH2SERVER/FPFI/bin/Release/netcoreapp2.1/linux-x64/publish/* /var/fpfi/
		systemctl start kestrel-fpfi.service
	EOF
}
dbUpdate() {
	sudo rsync -iauvzP --exclude 'bin' --exclude 'obj' --exclude 'Migrations' $PATH2FPFI root@$SERVER:$PATH2SERVER
	ssh $SERVER << EOF
		cd $PATH2SERVER/FPFI/
		dotnet restore
		dotnet ef migrations add $MIGRATION
		dotnet ef database update
		rm -R $PATH2SERVER/FPFI/bin/Release/netcoreapp2.1/linux-x64/
		dotnet publish -r linux-x64 -c Release
		systemctl stop kestrel-fpfi.service
		rm -R /var/fpfi/[A-Za-z]*
		rsync -iauvzP $PATH2SERVER/FPFI/bin/Release/netcoreapp2.1/linux-x64/publish/* /var/fpfi/
		systemctl start kestrel-fpfi.service
	EOF
}
install() {
	sudo rsync -iauvzP --exclude 'bin' --exclude 'obj' --exclude 'Migrations' $PATH2FPFI root@$SERVER:$PATH2SERVER
	ssh $SERVER << EOF
		cd $PATH2SERVER
		//install dotnet
		wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
		sudo dpkg -i packages-microsoft-prod.deb
		sudo add-apt-repository universe
		sudo apt install apt-transport-https
		sudo apt update
		sudo apt install dotnet-sdk-2.2
		//install mssql
		wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
		sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/16.04/mssql-server-2017.list)"
		sudo apt update
		sudo apt install -y mssql-server
		sudo /opt/mssql/bin/mssql-conf setup
		//install sqlcmd
		curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
		curl https://packages.microsoft.com/config/ubuntu/16.04/prod.list | sudo tee /etc/apt/sources.list.d/msprod.list
		sudo apt-get update
		sudo apt install mssql-tools unixodbc-dev
		echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bash_profile
		echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
		source ~/.bashrc
		//install nginx
		sudo apt install nginx
		sudo service nginx start
		sudo cp nginx /etc/nginx/sites-available/default
		sudo cp kestrel /etc/systemd/system/kestrel-fpfi.service
		sudo systemctl enable kestrel-fpfi.service
		//install ufw
		sudo apt install ufw
		//compile solution
		dotnet restore
		dotnet build
		dotnet ef database drop
		dotnet ef migrations add Initial
		//sqlcmd -S localhost -U SA -P $SQLPASS -Q 'CREATE DATABASE $NAME'
		dotnet ef database update
		//dotnet run
		dotnet publish -r linux-x64 -c Release
		dotnet publish -r linux-x64 -c Release
		sudo rm -R /var/fpfi/
		sudo mkdir /var/fpfi/
		sudo cp -R bin/Release/netcoreapp2.1/linux-x64/publish/* /var/fpfi/
		sudo chown -R guillermo /var/fpfi/
		systemctl start kestrel-fpfi.service
	EOF
}
sslUpdate(){
	cd $PATH2FPFI
	openssl pkcs12 -export -out fpfi.pfx -inkey fpfi.key -in fpfi.crt -certfile ca_bundle.crt
}

#########################
# The command line help #
#########################
display_help() {
    echo "Usage: $0 [variables...] {update|dbUpdate|install|sslUpdate}" >&2
    echo
	echo "   -n, --name					"
    echo "   -p, --path2fpfi			Local path to source code					(all)"
    echo "   -s, --server				Server domain e.g. 190.0.0.4 | lp2.fpfi.cl	{update|dbUpdate|install}"
    echo "   -d, --destination			Server path to store source code			{update|dbUpdate|install}"
    echo "   -m, --migration			Meaningfull name for DB migration			{dbUpdate}"
    echo
    # echo some stuff here for the -a or --add-options 
    exit 1
}

################################
# Check if parameters options  #
# are given on the commandline #
################################
while :
do
    case "$1" in
      -p | --path2fpfi)
          if [ $# -ne 0 ]; then
            PATH2FPFI="$2"   # You may want to check validity of $2
          fi
          shift 2
          ;;
      -h | --help)
          display_help  # Call your function
          exit 0
          ;;
      -s | --server)
          SERVER="$2"
           shift 2
           ;;

      -d | --destination)
          # do something here call function
          # and write it in your help function display_help()
		  PATH2SERVER="$2"
          shift 2
          ;;

	  -m | --migration)
          # do something here call function
          # and write it in your help function display_help()
		  MIGRATION="$2"
          shift 2
          ;;

      --) # End of all options
          shift
          break
          ;;
      -*)
          echo "Error: Unknown option: $1" >&2
          ## or call function display_help
          exit 1 
          ;;
      *)  # No more options
          break
          ;;
    esac
done

###################### 
# Check if parameter #
# is set too execute #
######################
case "$1" in
  update)
    update # calling function start()
    ;;
  dbUpdate)
    dbUpdate # calling function stop()
    ;;
  install)
    install  # calling function stop()
    ;;
  sslUpdate)
    sslUpdate  # calling function stop()
    ;;
  *)
#    echo "Usage: $0 {start|stop|restart}" >&2
     display_help

exit 1
;;