(function () {
    var job = null;
    var isMining = false;
    var user = '';
    var worker = '';
    var hashRate = 0;
    var messages = [];
    var tempOperations = 0;
    var tempStartClock = 0;

    var submitJob = function (job) {
        console.warn('Not initialized yet!');
    }

    $(function () {
        $('#start-btn').click(function () {
            user = $('#user-input').val();
            worker = $('#worker-input').val();

            $('#user').text(user);
            $('#worker').text(worker);

            if (!user || !worker) {
                alert('User and worker are required!');
                return;
            }

            $('#startup-wrapper').remove();
            $('#mining-wrapper').show();

            mine();

            const connection = new signalR.HubConnectionBuilder()
                .withUrl('/poolhub?xuser=' + user + '&xworker=' + worker)
                .build();

            connection.on("NewJob", (data) => newJobHandler(data));

            connection.start().catch(err => console.error(err.toString()));

            setInterval(function () {
                if (hashRate) {
                    console.log('Report hashrate -> ' + hashRate);
                    connection.invoke('ReportHashrate', hashRate).catch(err => console.error(err.toString()));
                }
            }, 5000);

            submitJob = function (job) {
                console.log('submit job!');

                job.user = user;
                job.worker = worker;

                connection.invoke('SubmitJob', job).catch(err => console.error(err.toString()));
            }
        });
    });

    function mine() {
        if (isMining) {
            tempOperations++;
            job.nonce = getRandomNonce();
            job.blockHash = getJobBlockHash(job);
            if (isValidJob(job)) {
                isMining = false;
                updateHashRate();
                submitJob(job);
            }
        }

        setTimeout(mine, 0);
    }

    function pad(num) {
        var norm = Math.floor(Math.abs(num));
        return (norm < 10 ? '0' : '') + norm;
    }

    function getIsoDate(date) {
        var tzo = -date.getTimezoneOffset(),
            dif = tzo >= 0 ? '+' : '-';

        return date.getFullYear() +
            '-' + pad(date.getMonth() + 1) +
            '-' + pad(date.getDate()) +
            'T' + pad(date.getHours()) +
            ':' + pad(date.getMinutes()) +
            ':' + pad(date.getSeconds()) +
            dif + pad(tzo / 60) +
            ':' + pad(tzo % 60);
    }

    function updateHashRate() {
        var startedAt = tempStartClock;
        var operations = tempOperations;

        tempOperations = 0;
        tempStartClock = new Date();

        if (!startedAt) {
            return;
        }

        var elapsed = new Date() - startedAt;
        hashRate = operations / elapsed;
        $('#hashrate').text(hashRate);
    }

    function newJobHandler(newJob) {
        console.log('new job!');
        job = newJob;
        job.dateCreated = getIsoDate(new Date());

        updateHashRate();

        isMining = true;
    }

    function getJobBlockHash(job) {
        return CryptoJS.SHA256(job.blockDataHash + '|' + job.dateCreated + '|' + job.nonce).toString(CryptoJS.enc.Hex);
    }

    function isValidJob(job) {
        for (var i = 0; i < job.difficulty - 2; i++) {
            if (job.blockHash[i] !== '0') {
                return false;
            }
        }

        return true;
    }

    function getRandomNonce() {
        return Math.floor(Math.random() * 18446744073709551615) + 0;
    }
}()); 