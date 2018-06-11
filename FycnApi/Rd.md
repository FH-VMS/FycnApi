1.导出Excel
Ubuntu 16.04 and above:
	apt-get install libgdiplus
	cd /usr/lib
	ln -s libgdiplus.so gdiplus.dll
Fedora 23 and above:
	dnf install libgdiplus
	cd /usr/lib64/
	ln -s libgdiplus.so.0 gdiplus.dll
CentOS 7 and above:
	yum install autoconf automake libtool
	yum install freetype-devel fontconfig libXft-devel
	yum install libjpeg-turbo-devel libpng-devel giflib-devel libtiff-devel libexif-devel
	yum install glib2-devel cairo-devel
	git clone https://github.com/mono/libgdiplus
	cd libgdiplus
	./autogen.sh
	make
	make install
	cd /usr/lib64/
	ln -s /usr/local/lib/libgdiplus.so gdiplus.dll


	redis: 
	0:机器在线情况  订单是否过期
	1:socket下推缓存
	2：后台缓存 例如client_id
	3:取货码缓存
	4：微信鉴权